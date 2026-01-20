using CSharpFunctionalExtensions;
using SeatReservation.Contracts;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public class CreateVenueHandler
{
    private readonly IVenuesRepository _venuesRepository;

    public CreateVenueHandler(IVenuesRepository venuesRepository)
    {
        _venuesRepository = venuesRepository;
    }

    /// <summary>
    /// Создание площадки со всеми местами
    /// </summary>
    public async Task<Result<Guid, Error>> Handle(CreateVenueRequest request, CancellationToken cancellationToken)
    {
        var createVenueResult = Venue.Create(null, request.Prefix, request.Name, request.SeatsLimit);

        if (createVenueResult.IsFailure)
        {
            return createVenueResult.Error;
        }

        var venue = createVenueResult.Value;

        foreach (var seatRequest in request.Seats)
        {
            var createSeatResult = Seat.Create(venue, seatRequest.RowNumber, seatRequest.SeatNumber);

            if (createSeatResult.IsFailure)
            {
                return createSeatResult.Error;
            }

            var addSeatResult = venue.AddSeat(createSeatResult.Value);

            if (addSeatResult.IsFailure)
            {
                return addSeatResult.Error;
            }
        }

        await _venuesRepository.Add(venue, cancellationToken);

        return venue.Id.Value;
    }
}