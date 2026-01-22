using CSharpFunctionalExtensions;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Application.Venues;

public interface IVenuesRepository
{
    Task<Result<Venue, Error>> GetByIdAsync(VenueId venueId, CancellationToken cancellationToken = default);
    Task<Result<Venue, Error>> GetByIdWithSeatsAsync(VenueId venueId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Venue>> GetByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    Task<Result<Guid, Error>> AddAsync(Venue venue, CancellationToken cancellationToken = default);
    Task<Result<Guid, Error>> UpdateVenueNameAsync(VenueId id, VenueName name, CancellationToken cancellationToken = default);
    Task<UnitResult<Error>> UpdateVenueNameByPrefixAsync(string prefix, VenueName name, CancellationToken cancellationToken = default);
    Task<UnitResult<Error>> DeleteSeatsByVenueIdAsync(VenueId venueId, CancellationToken cancellationToken = default);
}