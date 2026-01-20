using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeatReservation.Domain;
using SeatReservation.Domain.Venues;

namespace SeatReservation.Infrastructure.Postgres.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.ToTable("venues");

        builder.HasKey(x => x.Id).HasName("pk_venues");

        builder.Property(x => x.Id).HasColumnName("id")
            .HasConversion(venueId => venueId.Value, id => new VenueId(id));

        builder.ComplexProperty(x => x.Name, nameBuilder =>
        {
            nameBuilder.Property(x => x.Prefix)
                .HasColumnName("prefix")
                .HasMaxLength(LengthConstants.LENGTH_50)
                .IsRequired();
            nameBuilder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(LengthConstants.LENGTH_500)
                .IsRequired();
        });

        builder.Property(x => x.SeatsLimit).HasColumnName("seats_limit").IsRequired();

        builder.HasMany(x => x.Seats)
            .WithOne(x => x.Venue)
            .HasForeignKey(s => s.VenueId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}