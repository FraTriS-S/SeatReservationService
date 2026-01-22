using CSharpFunctionalExtensions;
using SeatReservation.Application.Events;
using SeatReservation.Application.Seats;
using SeatReservation.Contracts;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Reservations;

public class CreateReservationHandler
{
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly ISeatsRepository _seatsRepository;

    public CreateReservationHandler(IReservationsRepository reservationsRepository, IEventsRepository eventsRepository, ISeatsRepository seatsRepository)
    {
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
        _seatsRepository = seatsRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        // 1. доступно ли мероприятие для бронирования (даты, статус)

        var eventId = new EventId(request.EventId);

        var (_, isFailure, @event, error) = await _eventsRepository.GetByIdAsync(eventId, cancellationToken);

        if (isFailure)
        {
            return error;
        }

        if (!@event.IsAvailableForReservation())
        {
            return Error.Failure("reservation.unavailable", "Reservation is unavailable");
        }

        // 2. места принадлежат мероприятию и площадке

        var seatIds = request.SeatIds.Select(x => new SeatId(x)).ToList();

        var seats = await _seatsRepository.GetByIdsAsync(seatIds, cancellationToken);

        if (seats.Any(seat => seat.VenueId != @event.VenueId) || seats.Count == 0)
        {
            return Error.Conflict("seat.conflict", "Seat does not belong to venue");
        }

        // 3. места не забронированны

        var isSeatsReserved = await _reservationsRepository.AnySeatsAlreadyReserved(request.EventId, seatIds, cancellationToken);

        if (isSeatsReserved)
        {
            return Error.Conflict("seat.conflict", "Seats already reserved");
        }

        //
        var (_, isFailureCreate, reservation, createError) = Reservation.Create(request.EventId, request.UserId, request.SeatIds);

        if (isFailureCreate)
        {
            return createError;
        }

        var (_, isFailureAdd, reservationId, addError) = await _reservationsRepository.AddAsync(reservation, cancellationToken);

        if (isFailureAdd)
        {
            return addError;
        }

        return reservationId;
    }
}