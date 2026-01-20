using CSharpFunctionalExtensions;
using SeatReservation.Contracts;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class UpdateVenueNameHandler
{
    private readonly IVenuesRepository _repository;

    public UpdateVenueNameHandler(IVenuesRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueNameRequest request, CancellationToken cancellationToken)
    {
        var venueId = new VenueId(request.Id);

        var venueResult = await _repository.GetByIdAsync(venueId, cancellationToken);

        if (venueResult.IsFailure)
        {
            return venueResult.Error;
        }

        var venue = venueResult.Value;

        venue.UpdateName(request.Name);

        await _repository.SaveAsync(cancellationToken);

        return venue.Id.Value;
    }
}