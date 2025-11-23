namespace SeatReservation.Domain.Reservations;

public class ReservationSeat
{
    public ReservationSeat(Guid id, Reservation reservation, Guid seatId)
    {
        Id = id;
        Reservation = reservation;
        SeatId = seatId;
        ReservedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Reservation Reservation { get; private set; }
    public Guid SeatId { get; private set; }
    public DateTime ReservedAt { get; }
}