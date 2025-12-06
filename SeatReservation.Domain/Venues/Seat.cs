using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Shared;

namespace SeatReservation.Domain.Venues;

public record SeatId(Guid Value);

public class Seat
{
    private Seat(SeatId id, int rowNumber, int seatNumber)
    {
        Id = id;
        RowNumber = rowNumber;
        SeatNumber = seatNumber;
    }

    public SeatId Id { get; } = null!;
    public int RowNumber { get; private set; }
    public int SeatNumber { get; private set; }
    public VenueId VenueId { get; private set; } = null!;

    public static Result<Seat, Error> Create(int rowNumber, int seatNumber)
    {
        if (rowNumber < 0 || seatNumber < 0)
        {
            return Error.Validation("seat.rowNumber", "Row number cannot be negative", null);
        }

        return new Seat(new SeatId(Guid.NewGuid()), rowNumber, seatNumber);
    }

    [UsedImplicitly]
    private Seat()
    {
        // EF Core
    }
}