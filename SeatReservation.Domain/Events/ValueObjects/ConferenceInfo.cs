namespace SeatReservation.Domain.Events.ValueObjects;

public record ConferenceInfo(string Speaker, string Topic) : IEventInfo
{
    public override string ToString() => $"Conference: {Speaker}, Topic: {Topic}";
}