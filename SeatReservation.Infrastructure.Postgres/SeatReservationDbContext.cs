using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SeatReservation.Application.Database;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres;

public class SeatReservationDbContext : DbContext, IReadDbContext
{
    private readonly string _connectionString;

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<ReservationSeat> ReservationSeats => Set<ReservationSeat>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Seat> Seats => Set<Seat>();

    public IQueryable<Event> EventsRead => Set<Event>().AsNoTracking();
    public IQueryable<Venue> VenuesRead => Set<Venue>().AsNoTracking();
    public IQueryable<Seat> SeatsRead => Set<Seat>().AsNoTracking();
    public IQueryable<Reservation> ReservationsRead => Set<Reservation>().AsNoTracking();
    public IQueryable<ReservationSeat> ReservationSeatsRead => Set<ReservationSeat>().AsNoTracking();

    public SeatReservationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);

        // только для разработки
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeatReservationDbContext).Assembly);

        modelBuilder.HasPostgresExtension("pg_trgm");
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}