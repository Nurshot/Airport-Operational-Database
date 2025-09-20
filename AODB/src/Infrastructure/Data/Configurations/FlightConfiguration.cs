using AODB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AODB.Infrastructure.Data.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.Property(t => t.FlightNumber)
            .HasMaxLength(10)
            .IsRequired();

        // Airline
        builder.HasOne(f => f.Airline)
            .WithMany() 
            .HasForeignKey(f => f.AirlineId)
            .OnDelete(DeleteBehavior.Cascade);

        // Aircraft  Flight
        builder.HasOne(f => f.Aircraft)
            .WithMany() 
            .HasForeignKey(f => f.AircraftId)
            .OnDelete(DeleteBehavior.Cascade);

        // Origin Airport
        builder.HasOne(f => f.OriginAirport)
            .WithMany() 
            .HasForeignKey(f => f.OriginAirportId)
            .OnDelete(DeleteBehavior.NoAction); 

        // Destination Airport 
        builder.HasOne(f => f.DestinationAirport)
            .WithMany() 
            .HasForeignKey(f => f.DestinationAirportId)
            .OnDelete(DeleteBehavior.NoAction); 

        // Enum Type
        builder.Property(f => f.FlightType)
            .HasConversion<int>();

        builder.Property(f => f.Category)
            .HasConversion<int>();

        builder.Property(f => f.Status)
            .HasConversion<int>();
    }
}
