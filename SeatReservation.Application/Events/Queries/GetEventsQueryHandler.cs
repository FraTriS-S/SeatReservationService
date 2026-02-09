using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Application.Events.Queries;

public class GetEventsQueryHandler
{
    private readonly IReadDbContext _dbContext;

    public GetEventsQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetEventsResponse> Handle(GetEventsQuery query, CancellationToken cancellationToken)
    {
        var eventsQuery = _dbContext.EventsRead;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            eventsQuery = eventsQuery.Where(@event => EF.Functions.Like(@event.Name.ToLower(), $"%{query.Search.ToLower()}%"));
        }

        if (!string.IsNullOrWhiteSpace(query.EventType))
        {
            eventsQuery = eventsQuery.Where(@event => @event.Type.ToString() == query.EventType);
        }

        if (query.DateFrom.HasValue)
        {
            eventsQuery = eventsQuery.Where(@event => @event.EventDate >= query.DateFrom.Value.ToUniversalTime());
        }

        if (query.DateTo.HasValue)
        {
            eventsQuery = eventsQuery.Where(@event => @event.EventDate <= query.DateTo.Value.ToUniversalTime());
        }

        if (query.VenueId.HasValue)
        {
            eventsQuery = eventsQuery.Where(@event => @event.VenueId == new VenueId(query.VenueId.Value));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            eventsQuery = eventsQuery.Where(@event => @event.Status.ToString() == query.Status);
        }

        if (query.MinAvailableSeats.HasValue)
        {
            eventsQuery = eventsQuery.Where(@event =>
                _dbContext.SeatsRead.Count(seat => seat.VenueId == @event.VenueId) -
                _dbContext.ReservationSeatsRead.Count(reservationSeat =>
                    reservationSeat.EventId == @event.Id &&
                    (reservationSeat.Reservation.Status == ReservationStatus.Confirmed ||
                     reservationSeat.Reservation.Status == ReservationStatus.Pending))
                >= query.MinAvailableSeats.Value);
        }

        eventsQuery = eventsQuery
            .OrderByDescending(@event => @event.EventDate)
            .ThenBy(@event => @event.Id);

        var totalCount = await eventsQuery.LongCountAsync(cancellationToken);

        eventsQuery = eventsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);


        var events = await eventsQuery
            .Select(@event => new EventDto
            {
                Id = @event.Id.Value,
                VenueId = @event.VenueId.Value,
                Name = @event.Name,
                EventDate = @event.EventDate,
                Capacity = @event.Details.Capacity,
                Description = @event.Details.Description,
                LastReservationDateTime = @event.Details.LastReservationDateTime,
                Type = @event.Type.ToString(),
                Info = @event.Info.ToString(),
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                Status = @event.Status.ToString(),
                TotalSeats = _dbContext.SeatsRead.Count(seat => seat.VenueId == @event.VenueId),
                ReservedSeats = _dbContext.ReservationSeatsRead.Count(reservationSeat => reservationSeat.EventId == @event.Id),
                AvailableSeats = _dbContext.SeatsRead.Count(seat => seat.VenueId == @event.VenueId) -
                                 _dbContext.ReservationSeatsRead.Count(reservationSeat => reservationSeat.EventId == @event.Id &&
                                                                                          (reservationSeat.Reservation.Status == ReservationStatus.Confirmed ||
                                                                                           reservationSeat.Reservation.Status == ReservationStatus.Pending))
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return new GetEventsResponse { Items = events, TotalCount = totalCount };
    }
}