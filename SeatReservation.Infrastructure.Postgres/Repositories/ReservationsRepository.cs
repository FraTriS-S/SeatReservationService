using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.Reservations;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;
using Shared;

namespace SeatReservation.Infrastructure.Postgres.Repositories;

public class ReservationsRepository : IReservationsRepository
{
    private readonly SeatReservationDbContext _dbContext;
    private readonly ILogger<ReservationsRepository> _logger;

    public ReservationsRepository(SeatReservationDbContext dbContext, ILogger<ReservationsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.Reservations.AddAsync(reservation, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return reservation.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail to insert reservation");

            return Error.Failure("reservation.insert", "Fail to insert reservation");
        }
    }

    public async Task<bool> AnySeatsAlreadyReserved(
        Guid eventId, IEnumerable<SeatId> seatIds, CancellationToken cancellationToken = default) =>
        await _dbContext.Reservations
            .Where(reservation => reservation.EventId == eventId)
            .Where(reservation => reservation.ReservedSeats.Any(reservationSeat => seatIds.Contains(reservationSeat.SeatId)))
            .AnyAsync(cancellationToken);

    public async Task<int> GetReservedSeatsCount(Guid eventId, CancellationToken cancellationToken = default)
    {
        // Пессимистичная блокировка (потому что всегда рассчитываем что нужно блокировать),
        // блокируем поле c ожиданием
        // await _dbContext.Database.ExecuteSqlAsync(
        //     $"SELECT capacity FROM events_details WHERE event_id = {eventId} FOR UPDATE", cancellationToken);
        // блокируем поле c ошибкой
        // await _dbContext.Database.ExecuteSqlAsync(
        //     $"SELECT capacity FROM events_details WHERE event_id = {eventId} FOR UPDATE NOWAIT", cancellationToken);

        return await _dbContext.Reservations
            .Where(reservation => reservation.EventId == eventId)
            .Where(reservation => reservation.Status == ReservationStatus.Confirmed || reservation.Status == ReservationStatus.Pending)
            .SelectMany(x => x.ReservedSeats)
            .CountAsync(cancellationToken);
    }
}