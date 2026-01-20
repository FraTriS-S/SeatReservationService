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

    private Venue(VenueId id, VenueName name, int seatsLimit)
    {
        Id = id;
        Name = name;
        SeatsLimit = seatsLimit;
    }

    public VenueId Id { get; } = null!;
    public VenueName Name { get; private set; } = null!;
    public int SeatsLimit { get; private set; }
    public int SeatsCount => _seats.Count;
    public IReadOnlyList<Seat> Seats => _seats;

    public static Result<Venue, Error> Create(VenueId? venueId, string prefix, string name, int seatsLimit)
    {
        if (seatsLimit <= 0)
        {
            return Error.Validation("venue.seatsLimit", "Seats limit must be greater than zero");
        }

        var venueNameResult = VenueName.Create(prefix, name);

        if (venueNameResult.IsFailure)
        {
            return venueNameResult.Error;
        }

        return new Venue(venueId ?? new VenueId(Guid.NewGuid()), venueNameResult.Value, seatsLimit);
    }

    public UnitResult<Error> AddSeat(Seat seat)
    {
        if (SeatsCount >= SeatsLimit)
        {
            return Error.Conflict("venue.seats.limit", "");
        }

        _seats.Add(seat);

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> UpdateSeats(IEnumerable<Seat> seats)
    {
        var seatsList = seats.ToList();

        if (seatsList.Count > SeatsLimit)
        {
            return Error.Conflict("venue.seats.limit", "");
        }

        _seats = seatsList;

        return UnitResult.Success<Error>();
    }

    public void ExpandSeatsLimit(int newSeatsLimit) => SeatsLimit = newSeatsLimit;

    public UnitResult<Error> UpdateName(string name)
    {
        var newVenueNameResult = VenueName.Create(Name.Prefix, name);

        if (newVenueNameResult.IsFailure)
        {
            return newVenueNameResult.Error;
        }

        Name = newVenueNameResult.Value;

        return UnitResult.Success<Error>();
    }

    [UsedImplicitly]
    private Venue()
    {
        // EF Core
    }
}