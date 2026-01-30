namespace SeatReservation.Domain.Events.ValueObjects;

public record ConcertInfo(string Performer) : IEventInfo
{
    public override string ToString() => $"Concert: {Performer}";
}