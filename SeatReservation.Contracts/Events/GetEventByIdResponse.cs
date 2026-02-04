using SeatReservation.Contracts.Seats;

namespace SeatReservation.Contracts.Events;

public class GetEventByIdResponse
{
    public Guid Id { get; init; }
    public Guid VenueId { get; init; }
    public string Name { get; init; } = null!;
    public DateTime EventDate { get; init; }
    public int Capacity { get; init; }
    public string Description { get; init; } = null!;
    public DateTime? LastReservationDateTime { get; init; }
    public string Type { get; init; } = null!;
    public string Info { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Status { get; init; } = null!;
    public List<AvailableSeatDto> Seats { get; init; } = [];
}