using CSharpFunctionalExtensions;
using SeatReservation.Contracts;
using Shared;

namespace SeatReservation.Application.Venues;

public class UpdateVenueNameByPrefixHandler
{
    private readonly IVenuesRepository _repository;

    public UpdateVenueNameByPrefixHandler(IVenuesRepository repository)
    {
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

        await _repository.SaveAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}