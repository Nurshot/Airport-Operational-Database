using AODB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AODB.Infrastructure.Data.Configurations;

public class FlightTimesHistoryConfiguration : IEntityTypeConfiguration<FlightTimesHistory>
{
    public void Configure(EntityTypeBuilder<FlightTimesHistory> builder)
    {
        //  Flight ile
        builder.HasOne(fth => fth.Flight)
            .WithMany(f => f.TimesHistory)
            .HasForeignKey(fth => fth.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        // ENUM
        builder.Property(fth => fth.UpdateType)
            .HasConversion<int>();

        builder.Property(fth => fth.UpdateSource)
            .HasConversion<int>();

        
        builder.Property(fth => fth.UpdateReason)
            .HasMaxLength(100);

        builder.Property(fth => fth.InsertedBy)
            .HasMaxLength(50);

        
        builder.Property(fth => fth.TimeValue).HasColumnType("datetime2");
        builder.Property(fth => fth.InsertedAt).HasColumnType("datetime2");
    }
}
