using AODB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AODB.Infrastructure.Data.Configurations;

public class FlightTimesCurrentConfiguration : IEntityTypeConfiguration<FlightTimesCurrent>
{
    public void Configure(EntityTypeBuilder<FlightTimesCurrent> builder)
    {
        // Primary Key = FlightId (One-to-One)
        // Zaten historyde tuttuğumuz için gerek yok burda ayrı Id tutmaya
        builder.HasKey(ftc => ftc.FlightId);

        // One-to-One ilişki
        builder.HasOne(ftc => ftc.Flight)
            .WithOne(f => f.CurrentTimes)
            .HasForeignKey<FlightTimesCurrent>(ftc => ftc.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        
        builder.Property(ftc => ftc.STA).HasColumnType("datetime2");
        builder.Property(ftc => ftc.ETA).HasColumnType("datetime2");
        builder.Property(ftc => ftc.ATA).HasColumnType("datetime2");
        builder.Property(ftc => ftc.STD).HasColumnType("datetime2");
        builder.Property(ftc => ftc.ETD).HasColumnType("datetime2");
        builder.Property(ftc => ftc.ATD).HasColumnType("datetime2");
        builder.Property(ftc => ftc.OffBlockTime).HasColumnType("datetime2");
        builder.Property(ftc => ftc.InBlockTime).HasColumnType("datetime2");
        builder.Property(ftc => ftc.LastUpdated).HasColumnType("datetime2");
    }
}
