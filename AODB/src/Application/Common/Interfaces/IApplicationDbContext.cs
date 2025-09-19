using AODB.Domain.Entities;

namespace AODB.Application.Common.Interfaces;

public interface IApplicationDbContext
{
  
    DbSet<Airline> Airlines { get; }

    DbSet<Airport> Airports { get; }
    DbSet<Aircraft> Aircrafts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
