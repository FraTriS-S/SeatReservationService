using CSharpFunctionalExtensions;
using SeatReservation.Application.Database;
using SeatReservation.Application.Events;
using SeatReservation.Application.Seats;
using SeatReservation.Contracts.Reservations;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Reservations.Commands;

public class ReserveAdjacentSeatsHandler
{
    private readonly ITransactionManager _transactionManager;
    private readonly IReservationsRepository _reservationsRepository;
    private readonly ISeatsRepository _seatsRepository;
    private readonly IEventsRepository _eventsRepository;

    public ReserveAdjacentSeatsHandler(
        ITransactionManager transactionManager,
        IReservationsRepository reservationsRepository,
        ISeatsRepository seatsRepository,
        IEventsRepository eventsRepository)
    {
        _transactionManager = transactionManager;
        _reservationsRepository = reservationsRepository;
        _seatsRepository = seatsRepository;
        _eventsRepository = eventsRepository;
    }

    public async Task<Result<Guid, Error>> Handle(ReserveAdjacentSeatsRequest request, CancellationToken cancellationToken)
    {
        if (request.RequiredSeatsCount <= 0)
        {
            return Error.Validation("reserveAdjacent.seatsCount", "Required seats count mast ve greater then zero");
        }

        if (request.RequiredSeatsCount > 10)
        {
            return Error.Validation("reserveAdjacent.seatsCount", "Can`t reserve more then 10 adjacent seats at once");
        }

        var beginTransactionResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (beginTransactionResult.IsFailure)
        {
            return beginTransactionResult.Error;
        }

        using var transactionScope = beginTransactionResult.Value;

        var eventId = new EventId(request.EventId);

        // для блокировки
        var (_, isFailure, _, error) = await _eventsRepository.GetByIdWithLockAsync(eventId, cancellationToken);

        if (isFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : error;
        }

        var venueId = new VenueId(request.VenueId);

        var availableSeats = await _seatsRepository.GetAvailableSeats(
            venueId, eventId, request.PreferredRowNumber, cancellationToken);

        if (availableSeats.Count == 0)
        {
            return Error.NotFound("reserveAdjacent.seats", "No available seats found");
        }

        var selectedSeats = request.PreferredRowNumber.HasValue
            ? AdjacentSeatsFinder.FindAdjacentSeatsInPreferredRow(availableSeats, request.RequiredSeatsCount, request.PreferredRowNumber.Value)
            : AdjacentSeatsFinder.FindBestAdjacentSeats(availableSeats, request.RequiredSeatsCount);

        if (selectedSeats.Count == 0)
        {
            return Error.NotFound(
                "reserveAdjacent.seats",
                $"Could not find {request.RequiredSeatsCount} adjacent available seats");
        }

        if (selectedSeats.Count < request.RequiredSeatsCount)
        {
            return Error.NotFound(
                "reserveAdjacent.seats",
                $"Only {selectedSeats.Count} adjacent seats available? but {request.RequiredSeatsCount} requiered");
        }

        var seatIds = selectedSeats.Select(x => x.Id).ToList();

        var (_, createReservationFailure, reservation, createReservationError) = Reservation.Create(
            request.EventId,
            request.UserId,
            seatIds.Select(x => x.Value));

        if (createReservationFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? createReservationError
                : error;
        }

        var addResult = await _reservationsRepository.AddAsync(reservation, cancellationToken);

        if (addResult.IsFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? addResult.Error
                : error;
        }

        var commitResult = transactionScope.Commit();
        if (commitResult.IsFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? commitResult.Error
                : error;
        }

        return addResult.Value;
    }
}