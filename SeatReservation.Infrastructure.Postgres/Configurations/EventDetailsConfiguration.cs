using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Events;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class EventDetailsConfiguration : IEntityTypeConfiguration<EventDetails>
{
    public void Configure(EntityTypeBuilder<EventDetails> builder)
    {
        builder.ToTable("events_details");

        builder.HasKey(x => x.EventId).HasName("pk_events_details");

        builder.Property(x => x.EventId).HasColumnName("event_id")
            .HasConversion(eventId => eventId.Value, id => new EventId(id));
        builder.Property(x => x.Capacity).HasColumnName("capacity");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.LastReservationDateTime).HasColumnName("last_reservation_date_time");

        builder.Property(b => b.Version).IsRowVersion();

        builder.HasOne<Event>()
            .WithOne(x => x.Details)
            .HasForeignKey<EventDetails>(x => x.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}