using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Reservations;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class ReservationSeatConfiguration : IEntityTypeConfiguration<ReservationSeat>
{
    public void Configure(EntityTypeBuilder<ReservationSeat> builder)
    {
        builder.ToTable("reservation_seats");

        builder.HasKey(x => x.Id).HasName("pk_reservation_seats");

        builder.Property(x => x.Id).HasColumnName("id")
            .HasConversion(reservationSeatId => reservationSeatId.Value, id => new ReservationSeatId(id));

        builder.Property(x => x.SeatId).HasColumnName("seat_id").IsRequired();
        builder.Property(x => x.EventId).HasColumnName("event_id").IsRequired();
        builder.Property(x => x.ReservedAt).HasColumnName("reserved_at").IsRequired();

        builder.HasOne(x => x.Reservation)
            .WithMany(x => x.ReservedSeats)
            .HasForeignKey("reservation_id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Seat>()
            .WithMany()
            .HasForeignKey(x => x.SeatId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.EventId, x.SeatId }).IsUnique();
    }
}