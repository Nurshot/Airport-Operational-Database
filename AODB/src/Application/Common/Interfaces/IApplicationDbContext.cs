using AODB.Domain.Entities;

namespace AODB.Application.Common.Interfaces;

public interface IApplicationDbContext
{
  
    DbSet<Airline> Airlines { get; }

    DbSet<Airport> Airports { get; }
    DbSet<Aircraft> Aircrafts { get; }

    DbSet<Flight> Flights { get; }
    DbSet<FlightTimesCurrent> FlightTimesCurrent { get; }
    DbSet<FlightTimesHistory> FlightTimesHistory { get; }
    DbSet<Resource> Resources { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
