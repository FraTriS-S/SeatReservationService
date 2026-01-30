using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.Database;
using SeatReservation.Contracts.Events;
using SeatReservation.Domain.Events;

namespace SeatReservation.Application.Events.Queries;

public class GetEventByIdQueryHandler
{
    private readonly IReadDbContext _dbContext;

    public GetEventByIdQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetEventByIdResponse?> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
    {
        var @event = await _dbContext.EventsRead
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == new EventId(query.EventId), cancellationToken);

        if (@event == null)
        {
            return null;
        }

        return new GetEventByIdResponse
        {
            Id = @event.Id.Value,
            VenueId = @event.VenueId.Value,
            Name = @event.Name,
            EventDate = @event.EventDate,
            Capacity = @event.Details.Capacity,
            Description = @event.Details.Description,
            LastReservationDateTime = @event.Details.LastReservationDateTime,
            Type = @event.Type.ToString(),
            Info = @event.Info.ToString(),
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            Status = @event.Status.ToString(),
        };
    }
}