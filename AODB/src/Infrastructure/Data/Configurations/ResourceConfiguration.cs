using AODB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AODB.Infrastructure.Data.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        // Flight ile
        builder.HasOne(r => r.Flight)
            .WithMany(f => f.Resources)
            .HasForeignKey(r => r.FlightId)
            .OnDelete(DeleteBehavior.Cascade);

        
        builder.Property(r => r.Gate)
            .HasMaxLength(10);

        builder.Property(r => r.Stand)
            .HasMaxLength(10);

        builder.Property(r => r.BaggageBelt)
            .HasMaxLength(10);

        builder.Property(r => r.CheckInDesk)
            .HasMaxLength(20);
    }
}
