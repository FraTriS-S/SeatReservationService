using CSharpFunctionalExtensions;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application;

public interface IVenuesRepository
{
    Task<Result<Guid, Error>> Add(Venue venue, CancellationToken cancellationToken = default);
}