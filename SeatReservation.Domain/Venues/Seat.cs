using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Shared;

namespace SeatReservation.Domain.Venues;

public record SeatId(Guid Value);

public class Seat
{
    private Seat(SeatId id, Venue venue, int rowNumber, int seatNumber)
    {
        Id = id;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
        Venue = venue;
    }

    private Seat(SeatId id, VenueId venueId, int rowNumber, int seatNumber)
    {
        Id = id;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
        VenueId = venueId;
    }

    public SeatId Id { get; } = null!;
    public int RowNumber { get; private set; }
    public int SeatNumber { get; private set; }
    public Venue Venue { get; private set; } = null!;
    public VenueId VenueId { get; private set; } = null!;

    public static Result<Seat, Error> Create(Venue venue, int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seat.rowNumber", "Row number cannot be negative", null);
        }

        return new Seat(new SeatId(Guid.NewGuid()), venue, rowNumber, seatNumber);
    }

    public static Result<Seat, Error> Create(VenueId venueId, int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seat.rowNumber", "Row number cannot be negative", null);
        }

        return new Seat(new SeatId(Guid.NewGuid()), venueId, rowNumber, seatNumber);
    }

    [UsedImplicitly]
    private Seat()
    {
        // EF Core
    }
}