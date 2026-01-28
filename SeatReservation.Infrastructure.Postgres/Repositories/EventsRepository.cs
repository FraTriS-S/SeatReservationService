using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.Events;
using SeatReservation.Domain.Events;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class EventsRepository : IEventsRepository
{
    private readonly SeatReservationDbContext _dbContext;

    public EventsRepository(SeatReservationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Event, Error>> GetByIdAsync(EventId eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _dbContext.Events
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == eventId, cancellationToken);

        if (@event is null)
        {
            return Error.NotFound("@event.not.found", "Event not found");
        }

        return @event;
    }

    public async Task<Result<Event, Error>> GetByIdWithLockAsync(EventId eventId, CancellationToken cancellationToken = default)
    {
        var @event = await _dbContext.Events
            .FromSql($"SELECT * FROM events WHERE id = {eventId.Value} FOR UPDATE")
            .Include(x => x.Details)
            .FirstOrDefaultAsync(cancellationToken);

        if (@event is null)
        {
            return Error.NotFound("@event.not.found", "Event not found");
        }

        return @event;
    }
}