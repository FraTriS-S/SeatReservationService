using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Reservations;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(x => x.Id).HasName("pk_reservations");

        builder.Property(x => x.Id).HasColumnName("id")
            .HasConversion(reservationId => reservationId.Value, id => new ReservationId(id));
        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.Property(x => x.EventId).HasColumnName("event_id");
        builder.Property(x => x.Status).HasConversion<string>().HasColumnName("status").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => new { x.EventId, x.Status }).HasFilter("status IN ('Confirmed','Pending')");
    }
}