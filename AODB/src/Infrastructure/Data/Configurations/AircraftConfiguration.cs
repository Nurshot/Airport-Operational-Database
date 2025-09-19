using AODB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AODB.Infrastructure.Data.Configurations;

public class AircraftConfiguration : IEntityTypeConfiguration<Aircraft>
{
    public void Configure(EntityTypeBuilder<Aircraft> builder)
    {

       builder.Property(p => p.Registration)
             .IsRequired()
             .HasMaxLength(200)
             .HasColumnType("nvarchar(200)"); ;

        builder.Property(p => p.Type)
             .IsRequired()
             .HasMaxLength(100);

        // Foreign Key ilişkisi
        builder.HasOne(a => a.Airline)
               .WithMany() 
               .HasForeignKey(a => a.AirlineId)
               .OnDelete(DeleteBehavior.Restrict); // Aircraft varsa Airline silinemez

        
        builder.HasIndex(a => a.Registration)
               .IsUnique();


    }
}
