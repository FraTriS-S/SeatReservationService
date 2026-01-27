using CSharpFunctionalExtensions;
using SeatReservation.Application.Database;
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
    private readonly ITransactionManager _transactionManager;
    private readonly IReservationsRepository _reservationsRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly ISeatsRepository _seatsRepository;

    public CreateReservationHandler(
        ITransactionManager transactionManager,
        IReservationsRepository reservationsRepository,
        IEventsRepository eventsRepository,
        ISeatsRepository seatsRepository)
    {
        _transactionManager = transactionManager;
        _reservationsRepository = reservationsRepository;
        _eventsRepository = eventsRepository;
        _seatsRepository = seatsRepository;
    }

    public async Task<Result<Guid, Error>> Handle(CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (beginTransactionResult.IsFailure)
        {
            return beginTransactionResult.Error;
        }

        using var transactionScope = beginTransactionResult.Value;

        // 1. доступно ли мероприятие для бронирования (даты, статус)

        var eventId = new EventId(request.EventId);

        var (_, isFailure, @event, error) = await _eventsRepository.GetByIdAsync(eventId, cancellationToken);

        if (isFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : error;
        }

        var reservedSeatsCount = await _reservationsRepository.GetReservedSeatsCount(@event.Id.Value, cancellationToken);

        if (!@event.IsAvailableForReservation(reservedSeatsCount + request.SeatIds.Count()))
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : Error.Failure("reservation.unavailable", "Reservation is unavailable");
        }

        // 2. места принадлежат мероприятию и площадке

        var seatIds = request.SeatIds.Select(x => new SeatId(x)).ToList();

        var seats = await _seatsRepository.GetByIdsAsync(seatIds, cancellationToken);

        if (seats.Any(seat => seat.VenueId != @event.VenueId) || seats.Count == 0)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : Error.Conflict("seat.conflict", "Seat does not belong to venue");
        }

        var (_, isFailureCreate, reservation, createError) = Reservation.Create(request.EventId, request.UserId, request.SeatIds);

        if (isFailureCreate)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : createError;
        }

        var (_, isFailureAdd, reservationId, addError) = await _reservationsRepository.AddAsync(reservation, cancellationToken);

        if (isFailureAdd)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : addError;
        }

        // для оптимистичной блокировки
        @event.Details.ReserveSeat();

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : saveChangesResult.Error;
        }

        var commitResult = transactionScope.Commit();

        if (commitResult.IsFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : commitResult.Error;
        }

        return reservationId;
    }
}