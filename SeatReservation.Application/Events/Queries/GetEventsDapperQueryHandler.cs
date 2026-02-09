using System.Data;
using Dapper;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Events;

namespace SeatReservation.Application.Events.Queries;

public class GetEventsDapperQueryHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GetEventsDapperQueryHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<GetEventsResponse> Handle(GetEventsQuery query, CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);

        var parameters = new DynamicParameters();

        parameters.Add("@search", query.Search, DbType.String);
        parameters.Add("@event_type", query.EventType, DbType.String);
        parameters.Add("@date_from", query.DateFrom, DbType.DateTime);
        parameters.Add("@date_to", query.DateTo, DbType.DateTime);
        parameters.Add("@venue_id", query.VenueId, DbType.Guid);
        parameters.Add("@status", query.Status, DbType.String);
        parameters.Add("@min_available_seats", query.MinAvailableSeats, DbType.Int32);

        parameters.Add("@offset", (query.Page - 1) * query.PageSize, DbType.Int32);
        parameters.Add("@page_size", query.PageSize, DbType.Int32);

        List<string> conditions = [];

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            conditions.Add("events.name ILIKE @search");
            parameters.Add("search", $"%{query.Search}%");
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            conditions.Add("events.status = @status");
            parameters.Add("status", query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            conditions.Add("events.type = @event_type");
            parameters.Add("event_type", query.EventType);
        }

        if (query.DateFrom.HasValue)
        {
            conditions.Add("events.event_date >= @date_from");
            parameters.Add("date_from", query.DateFrom.Value.ToUniversalTime());
        }

        if (query.DateTo.HasValue)
        {
            conditions.Add("events.event_date <= @date_to");
            parameters.Add("date_to", query.DateTo.Value.ToUniversalTime());
        }

        if (query.VenueId.HasValue)
        {
            conditions.Add("events.venue_id = @venue_id");
            parameters.Add("venue_id", query.VenueId.Value);
        }

        if (query.MinAvailableSeats.HasValue)
        {
            conditions.Add("""
                           ((SELECT COUNT(*) FROM seats WHERE seats.venue_id = events.venue_id) - 
                            COALESCE((SELECT COUNT(*)
                                      FROM reservation_seats
                                               JOIN reservations ON reservation_seats.reservation_id = reservations.id
                                      WHERE reservation_seats.event_id = events.id
                                        AND reservations.status IN ('Confirmed', 'Pending')), 0)) >= @min_available_seats
                           """);
            parameters.Add("min_available_seats", query.MinAvailableSeats.Value);
        }

        parameters.Add("offset", (query.Page - 1) * query.PageSize);
        parameters.Add("page_size", query.PageSize);

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

        long? totalCount = null;

        var result = await connection.QueryAsync<EventDto, long, EventDto>(
            $"""
             SELECT events.id,
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
             (SELECT COUNT(*)
             FROM seats
             WHERE seats.venue_id = events.venue_id)                                                             as total_seats,
             (SELECT COUNT(*)
             FROM reservation_seats
             JOIN reservations ON reservations.id = reservation_seats.reservation_id
             WHERE reservation_seats.event_id = events.id
             AND reservations.status in ('Confirmed', 'Pending'))                                              as reserved_seats,
             ((SELECT COUNT(*)
             FROM seats
             WHERE seats.venue_id = events.venue_id) - (SELECT COUNT(*)
             FROM reservation_seats
             JOIN reservations ON reservations.id = reservation_seats.reservation_id
             WHERE reservation_seats.event_id = events.id
             AND reservations.status in ('Confirmed', 'Pending'))) as available_seats,
             COUNT(*) OVER () as total_count
             FROM events
             JOIN events_details ON events.id = events_details.event_id
             {whereClause}
                 ORDER BY events.event_date DESC, events.id
                 LIMIT @page_size OFFSET @offset
             """,
            splitOn: "total_count",
            map: (@event, count) =>
            {
                totalCount ??= count;
                return @event;
            },
            param: parameters
        );
        return new GetEventsResponse { Items = result.ToList(), TotalCount = totalCount ?? 0 };
    }
}