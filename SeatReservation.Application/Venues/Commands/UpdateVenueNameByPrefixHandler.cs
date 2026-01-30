using CSharpFunctionalExtensions;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Venues;
using Shared;

namespace SeatReservation.Application.Venues.Commands;

public class UpdateVenueNameByPrefixHandler
{
    private readonly ITransactionManager _transactionManager;
    private readonly IVenuesRepository _repository;

    public UpdateVenueNameByPrefixHandler(ITransactionManager transactionManager, IVenuesRepository repository)
    {
        _transactionManager = transactionManager;
        _repository = repository;
    }

    public async Task<UnitResult<Error>> Handle(UpdateVenueNameByPrefixRequest request, CancellationToken cancellationToken)
    {
        var venues = await _repository.GetByPrefixAsync(request.Prefix, cancellationToken);

        foreach (var venue in venues)
        {
            var updateNameResult = venue.UpdateName(request.Name);

            if (updateNameResult.IsFailure)
            {
                return updateNameResult;
            }
        }

        var saveChangesResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsFailure)
        {
            return saveChangesResult.Error;
        }

        return UnitResult.Success<Error>();
    }
}