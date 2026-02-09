namespace SeatReservation.Contracts.Events;

public record GetEventsQuery : PaginationRequest
{
    public string? Search { get; init; }
    public string? EventType { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public string? Status { get; init; }
    public Guid? VenueId { get; init; }
    public int? MinAvailableSeats { get; init; }
    public string? SortBy { get; init; } = "date";
    public string? SortDirection { get; init; } = "asc";
}