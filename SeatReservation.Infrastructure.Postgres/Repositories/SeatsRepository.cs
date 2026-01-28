using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.Seats;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class SeatsRepository : ISeatsRepository
{
    private readonly SeatReservationDbContext _dbContext;

    public SeatsRepository(SeatReservationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Seat>> GetByIdsAsync(IEnumerable<SeatId> seatIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Seats
            .Where(x => seatIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Seat>> GetAvailableSeats(
        VenueId venueId,
        EventId eventId,
        int? rowNumber,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Seats
            .Where(s => s.VenueId == venueId);

        if (rowNumber.HasValue)
        {
            query = query.Where(s => s.RowNumber == rowNumber.Value);
        }

        return await query.Where(s => !_dbContext.ReservationSeats
                .Any(rs => rs.SeatId == s.Id &&
                           rs.EventId == eventId.Value &&
                           (rs.Reservation.Status == ReservationStatus.Confirmed ||
                            rs.Reservation.Status == ReservationStatus.Pending)))
            .ToListAsync(cancellationToken);
    }
}