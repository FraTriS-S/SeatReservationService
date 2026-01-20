using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class EfCoreVenuesRepository : IVenuesRepository
{
    private readonly SeatReservationDbContext _dbContext;
    private readonly ILogger<EfCoreVenuesRepository> _logger;

    public EfCoreVenuesRepository(SeatReservationDbContext dbContext, ILogger<EfCoreVenuesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Venue, Error>> GetByIdAsync(VenueId venueId, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .FirstOrDefaultAsync(x => x.Id == venueId, cancellationToken);

        if (venue is null)
        {
            return Error.NotFound("venue.not.found", "Venue not found");
        }

        return venue;
    }

    public async Task<Result<Venue, Error>> GetByIdWithSeatsAsync(VenueId venueId, CancellationToken cancellationToken = default)
    {
        var venue = await _dbContext.Venues
            .Include(x => x.Seats)
            .FirstOrDefaultAsync(x => x.Id == venueId, cancellationToken);

        if (venue is null)
        {
            return Error.NotFound("venue.not.found", "Venue not found");
        }

        return venue;
    }

    public async Task<IReadOnlyList<Venue>> GetByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var venues = await _dbContext.Venues
            .Where(x => x.Name.Prefix.StartsWith(prefix))
            .ToListAsync(cancellationToken);

        return venues;
    }

    public async Task<Result<Guid, Error>> Add(Venue venue, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Venues.AddAsync(venue, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return venue.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to insert venue");

            return Error.Failure("venue.insert", "Fail to insert venue");
        }
    }

    public async Task<Result<Guid, Error>> UpdateVenueName(VenueId id, VenueName name, CancellationToken cancellationToken = default)
    {
        // Сохранение идет сразу через ExecuteUpdate
        await _dbContext.Venues
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.Name.Name, name.Name),
                cancellationToken);

        return id.Value;
    }

    public async Task<UnitResult<Error>> UpdateVenueNameByPrefix(string prefix, VenueName name, CancellationToken cancellationToken = default)
    {
        // Сохранение идет сразу через ExecuteUpdate
        await _dbContext.Venues
            .Where(x => x.Name.Prefix.StartsWith(prefix))
            .ExecuteUpdateAsync(setter =>
                    setter.SetProperty(x => x.Name.Name, name.Name),
                cancellationToken);

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> DeleteSeatsByVenueIdAsync(VenueId venueId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Seats
            .Where(x => x.Venue.Id == venueId)
            .ExecuteDeleteAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}