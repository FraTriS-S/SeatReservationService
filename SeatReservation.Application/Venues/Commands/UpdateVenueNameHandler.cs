using CSharpFunctionalExtensions;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Venues;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues.Commands;

public class UpdateVenueNameHandler
{
    private readonly ITransactionManager _transactionManager;
    private readonly IVenuesRepository _repository;

    public UpdateVenueNameHandler(ITransactionManager transactionManager, IVenuesRepository repository)
    {
        _transactionManager = transactionManager;
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueNameRequest request, CancellationToken cancellationToken)
    {
        var venueId = new VenueId(request.Id);

        var (_, isFailure, venue, error) = await _repository.GetByIdAsync(venueId, cancellationToken);

        if (isFailure)
        {
            return error;
        }

        var updateNameResult = venue.UpdateName(request.Name);

        if (updateNameResult.IsFailure)
        {
            return updateNameResult.Error;
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error;
        }

        return venue.Id.Value;
    }
}