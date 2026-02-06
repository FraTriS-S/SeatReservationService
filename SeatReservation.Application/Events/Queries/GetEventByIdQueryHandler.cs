using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Events;
using SeatReservation.Contracts.Seats;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;

namespace SeatReservation.Application.Events.Queries;

public class GetEventByIdQueryHandler
{
    private readonly IReadDbContext _dbContext;

    public GetEventByIdQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetEventByIdResponse?> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.EventsRead
            .Include(@event => @event.Details)
            .Where(@event => @event.Id == new EventId(query.EventId))
            .Select(@event => new GetEventByIdResponse
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
                TotalSeats = _dbContext.SeatsRead.Count(seats => seats.VenueId == @event.VenueId),
                ReservedSeats = _dbContext.ReservationSeatsRead.Count(reservationSeats => reservationSeats.EventId == @event.Id),
                AvailableSeats = _dbContext.SeatsRead.Count(seats => seats.VenueId == @event.VenueId) -
                                 _dbContext.ReservationSeatsRead.Count(reservationSeats => reservationSeats.EventId == @event.Id &&
                                                                                           (reservationSeats.Reservation.Status == ReservationStatus.Confirmed ||
                                                                                            reservationSeats.Reservation.Status == ReservationStatus.Pending)),
                Seats = (from seats in _dbContext.SeatsRead
                    where seats.VenueId == @event.VenueId
                    join reservationSeats in _dbContext.ReservationSeatsRead
                        on new { SeatId = seats.Id, EventId = @event.Id } equals new { SeatId = reservationSeats.SeatId, EventId = reservationSeats.EventId }
                        into reservations
                    from reservation in reservations.DefaultIfEmpty()
                    where @event.Id == new EventId(query.EventId)
                    orderby seats.RowNumber, seats.SeatNumber
                    select new AvailableSeatDto
                    {
                        Id = seats.Id.Value,
                        RowNumber = seats.RowNumber,
                        SeatNumber = seats.SeatNumber,
                        VenueId = seats.VenueId.Value,
                        IsAvailable = reservation == null
                    }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return @event ?? null;
    }
}