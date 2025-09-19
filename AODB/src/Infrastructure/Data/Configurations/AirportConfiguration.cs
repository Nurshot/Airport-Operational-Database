using AODB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AODB.Infrastructure.Data.Configurations;

public class AirportConfiguration : IEntityTypeConfiguration<Airport>
{
    public void Configure(EntityTypeBuilder<Airport> builder)
    {

        builder.Property(p => p.Name)
             .IsRequired()
             .HasMaxLength(200)
              .HasColumnType("nvarchar(200)"); 

        
        builder.Property(p => p.Iata_code)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength(); 

        
        builder.Property(p => p.Icao_code)
            .IsRequired()
            .HasMaxLength(4)
            .IsFixedLength();

        builder.HasIndex(p => p.Name)
       .IsUnique();

    }
}
