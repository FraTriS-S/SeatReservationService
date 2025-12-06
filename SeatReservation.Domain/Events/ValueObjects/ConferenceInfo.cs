namespace SeatReservation.Domain.Events.ValueObjects;

public record ConferenceInfo(string Speaker, string Topic) : IEventInfo;