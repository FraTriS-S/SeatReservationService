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

        builder.HasOne<Event>()
            .WithOne(x => x.Details)
            .HasForeignKey<EventDetails>(x => x.EventId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}