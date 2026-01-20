using CSharpFunctionalExtensions;
using SeatReservation.Contracts;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class UpdateVenueSeatsHandler
{
    private readonly IVenuesRepository _repository;

    public UpdateVenueSeatsHandler(IVenuesRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateVenueSeatsRequest request, CancellationToken cancellationToken)
    {
        var venueId = new VenueId(request.VenueId);

        var venueResult = await _repository.GetByIdAsync(venueId, cancellationToken);

        if (venueResult.IsFailure)
        {
            return venueResult.Error;
        }

        List<Seat> seats = [];
        foreach (var seatRequest in request.Seats)
        {
            var createSeatResult = Seat.Create(venueId, seatRequest.RowNumber, seatRequest.SeatNumber);

            if (createSeatResult.IsFailure)
            {
                return createSeatResult.Error;
            }

            seats.Add(createSeatResult.Value);
        }

        venueResult.Value.UpdateSeats(seats);

        await _repository.DeleteSeatsByVenueIdAsync(venueId, cancellationToken);

        await _repository.SaveAsync(cancellationToken);

        return venueId.Value;
    }
}