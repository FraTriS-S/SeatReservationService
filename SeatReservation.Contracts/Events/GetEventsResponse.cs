namespace SeatReservation.Contracts.Events;

public record GetEventsResponse
{
    public List<EventDto> Items { get; init; } = [];
    public long TotalCount { get; init; }
}