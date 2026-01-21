using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.Database;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class VenuesRepository : IVenuesRepository
{
    private readonly SeatReservationDbContext _dbContext;
    private readonly ILogger<VenuesRepository> _logger;

    public VenuesRepository(SeatReservationDbContext dbContext, ILogger<VenuesRepository> logger)
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
        // todo: сдедать через это хоть один метод для примера
        // await _dbContext.Database
        //     .ExecuteSqlRawAsync("UPDATE Venues SET name = @Name WHERE id = @Id",
        //     new NpgsqlParameter("@Name", name.Name),
        //     new NpgsqlParameter("@Id", id.Value));

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
        // Сохранение идет сразу через ExecuteDelete
        await _dbContext.Seats
            .Where(x => x.Venue.Id == venueId)
            .ExecuteDeleteAsync(cancellationToken);

        return UnitResult.Success<Error>();
    }
}