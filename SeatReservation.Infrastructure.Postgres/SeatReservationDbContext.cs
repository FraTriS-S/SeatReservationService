using Microsoft.EntityFrameworkCore;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres;

public class SeatReservationDbContext : DbContext
{
    private readonly string _connectionString;

    public SeatReservationDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeatReservationDbContext).Assembly);
    }

    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<Event> Events => Set<Event>();
}