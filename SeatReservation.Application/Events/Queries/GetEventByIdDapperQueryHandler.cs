using Dapper;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Events;
using SeatReservation.Contracts.Seats;

namespace SeatReservation.Application.Events.Queries;

public class GetEventByIdDapperQueryHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetEventByIdDapperQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetEventByIdResponse?> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        GetEventByIdResponse? getEventByIdResponse = null;
        var eventDto = await connection.QueryAsync<GetEventByIdResponse, AvailableSeatDto, GetEventByIdResponse>(
            """
            SELECT 
                events.id,
                events.venue_id,
                events.name,
                events.type,
                events.event_date,
                events.start_date,
                events.end_date,
                events.status,
                events.info,
                events_details.capacity,
                events_details.description,
                COUNT(*) OVER () as total_seats,
                COUNT(reservation_seats.seat_id) OVER () as reserved_seats,
                COUNT(*) OVER () - COUNT(reservation_seats.seat_id) OVER () as available_seats,
                seats.id,
                seats.venue_id,
                seats.row_number,
                seats.seat_number,
                reservation_seats is null as is_available
            FROM events
                     JOIN events_details ON events.id = events_details.event_id
                     JOIN seats ON seats.venue_id = events.venue_id
                     LEFT JOIN reservation_seats ON seats.id = reservation_seats.seat_id and reservation_seats.event_id = events.id
            WHERE events.id = @eventId
            ORDER BY seats.row_number, seats.seat_number
            """,
            param: new
            {
                eventId = query.EventId
            },
            splitOn: "id",
            map: (e, s) =>
            {
                getEventByIdResponse ??= e;
                getEventByIdResponse.Seats.Add(s);
                return getEventByIdResponse;
            });

        return eventDto.FirstOrDefault();
    }
}