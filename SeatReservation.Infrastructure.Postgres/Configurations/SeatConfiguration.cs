using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("seats");

        builder.HasKey(x => x.Id).HasName("pk_seats");

        builder.Property(x => x.Id).HasColumnName("id")
            .HasConversion(seatId => seatId.Value, id => new SeatId(id));

        builder.Property(x => x.VenueId).HasColumnName("venue_id");
    }
}