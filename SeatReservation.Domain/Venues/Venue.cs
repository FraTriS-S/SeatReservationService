using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Shared;

namespace SeatReservation.Domain.Venues;

public record VenueId(Guid Value);

/// <summary>
/// Площадка
/// </summary>
public class Venue
{
    private List<Seat> _seats = [];

    public Venue(VenueId id, VenueName name, int seatsLimit, IEnumerable<Seat> seats)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
        _seats = seats.ToList();
    }

    public VenueId Id { get; } = null!;
    public VenueName Name { get; private set; } = null!;
    public int SeatsLimit { get; private set; }
    public int SeatsCount => _seats.Count;
    public IReadOnlyList<Seat> Seats => _seats;

    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
        {
            return Error.Conflict("venue.seats.limit", "");
        }

        _seats.Add(seat);

        return UnitResult.Success<Error>();
    }

    public void ExpandSeatsLimit(int newSeatsLimit) => SeatsLimit = newSeatsLimit;

    [UsedImplicitly]
    private Venue()
    {
        // EF Core
    }
}