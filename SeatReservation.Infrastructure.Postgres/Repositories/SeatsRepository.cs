using Microsoft.EntityFrameworkCore;
using SeatReservation.Application.Seats;
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
}