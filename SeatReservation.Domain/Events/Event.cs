using JetBrains.Annotations;
using SeatReservation.Domain.Events.ValueObjects;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Domain.Events;

public record EventId(Guid Value);

public class Event
{
    public Event(
        EventId id,
        VenueId venueId,
        string name,
        DateTime eventDate,
        EventDetails eventDetails,
        EventType type,
        IEventInfo info)
    {
        Id = id;
        VenueId = venueId;
        Name = name;
        EventDate = eventDate;
        Details = eventDetails;
        Type = type;
        Info = info;
    }

    public EventId Id { get; } = null!;
    public VenueId VenueId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public DateTime EventDate { get; private set; }
    public EventDetails Details { get; private set; } = null!;
    public EventType Type { get; private set; }
    public IEventInfo Info { get; private set; } = null!;

    [UsedImplicitly]
    private Event()
    {
        // EF Core
    }
}