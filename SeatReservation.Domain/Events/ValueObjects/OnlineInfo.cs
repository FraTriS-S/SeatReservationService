namespace SeatReservation.Domain.Events.ValueObjects;

public record OnlineInfo(string Url) : IEventInfo;