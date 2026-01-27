using JetBrains.Annotations;

namespace SeatReservation.Domain.Events;

public class EventDetails
{
    public EventDetails(int capacity, string description)
    {
        Capacity = capacity;
        Description = description;
    }

    public EventId EventId { get; } = null!;
    public int Capacity { get; private set; }
    public string Description { get; private set; } = null!;
    public DateTime? LastReservationDateTime { get; private set; }
    public uint Version { get; private set; }

    public void ReserveSeat()
    {
        LastReservationDateTime = DateTime.UtcNow;
    }

    [UsedImplicitly]
    private EventDetails()
    {
        // EF Core
    }
}