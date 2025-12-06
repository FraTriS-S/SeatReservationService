using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain.Events;
using SeatReservation.Domain.Venues;
using SeatReservation.Infrastructure.Postgres.Converters;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("events");

        builder.HasKey(x => x.Id).HasName("pk_events");

        builder.Property(x => x.Id).HasColumnName("id")
            .HasConversion(eventId => eventId.Value, id => new EventId(id));

        builder.Property(x => x.VenueId).HasColumnName("venue_id");
        builder.Property(x => x.Type).HasConversion<string>().HasColumnName("type").IsRequired();
        builder.Property(x => x.Info).HasConversion(new EventInfoValueConverter()).HasColumnName("info").IsRequired();

        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(x => x.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}