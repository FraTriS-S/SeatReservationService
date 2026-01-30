using CSharpFunctionalExtensions;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Venues;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues.Commands;

public class UpdateVenueSeatsHandler
{
    private readonly ITransactionManager _transactionManager;
    private readonly IVenuesRepository _repository;

    public UpdateVenueSeatsHandler(ITransactionManager transactionManager, IVenuesRepository repository)
    {
        _transactionManager = transactionManager;
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueSeatsRequest request, CancellationToken cancellationToken)
    {
        var transactionScopeResult = await _transactionManager.BeginTransactionAsync(cancellationToken);

        if (transactionScopeResult.IsFailure)
        {
            return transactionScopeResult.Error;
        }

        using var transactionScope = transactionScopeResult.Value;

        var venueId = new VenueId(request.VenueId);

        var venueResult = await _repository.GetByIdAsync(venueId, cancellationToken);

        if (venueResult.IsFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : venueResult.Error;
        }

        List<Seat> seats = [];
        foreach (var seatRequest in request.Seats)
        {
            var createSeatResult = Seat.Create(venueId, seatRequest.RowNumber, seatRequest.SeatNumber);

            if (createSeatResult.IsFailure)
            {
                var rollbackResult = transactionScope.Rollback();

                return rollbackResult.IsFailure
                    ? rollbackResult.Error
                    : createSeatResult.Error;
            }

            seats.Add(createSeatResult.Value);
        }

        var updateSeatsResult = venueResult.Value.UpdateSeats(seats);

        if (updateSeatsResult.IsFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : updateSeatsResult.Error;
        }

        var deleteSeatsByVenueIdResult = await _repository.DeleteSeatsByVenueIdAsync(venueId, cancellationToken);

        if (deleteSeatsByVenueIdResult.IsFailure)
        {
            var rollbackResult = transactionScope.Rollback();

            return rollbackResult.IsFailure
                ? rollbackResult.Error
                : deleteSeatsByVenueIdResult.Error;
        }

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

        return venueId.Value;
    }
}