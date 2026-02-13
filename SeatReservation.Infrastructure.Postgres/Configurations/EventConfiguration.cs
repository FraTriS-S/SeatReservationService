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
            .HasConversion(eventId => eventId.Value, id => new EventId(id)).IsRequired();
        builder.Property(x => x.VenueId).HasColumnName("venue_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").IsRequired();
        builder.Property(x => x.EventDate).HasColumnName("event_date").IsRequired();
        builder.Property(x => x.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(x => x.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasColumnName("status").IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasColumnName("type").IsRequired();
        builder.Property(x => x.Info).HasConversion(new EventInfoValueConverter()).HasColumnName("info").IsRequired();

        builder.HasOne<Venue>()
            .WithMany()
            .HasForeignKey(x => x.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // на даты хорошо создавать индексы, так как высокая селективность
        builder.HasIndex(x => x.EventDate);

        // для ускорения фильтрации через Like
        builder.HasIndex(x => x.Name)
            .HasDatabaseName("ix_events_name_trgm")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");
    }
}