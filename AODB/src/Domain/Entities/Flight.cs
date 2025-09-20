using AODB.Domain.Enums;

namespace AODB.Domain.Entities;

public class Flight : BaseAuditableEntity
{
    public string FlightNumber { get; set; } = null!;
    
    // Foreign Keys
    public int AirlineId { get; set; }
    public int AircraftId { get; set; }
    public int OriginAirportId { get; set; }
    public int DestinationAirportId { get; set; }
    
    // Enums
    public FlightType FlightType { get; set; }
    public FlightCategory Category { get; set; }
    public FlightStatus Status { get; set; }
    
    // Navigation Properties (İlişkiler)
    public Airline Airline { get; set; } = null!;
    public Aircraft Aircraft { get; set; } = null!;
    public Airport OriginAirport { get; set; } = null!;
    public Airport DestinationAirport { get; set; } = null!;
    
    // One-to-One ilişki
    public FlightTimesCurrent? CurrentTimes { get; set; }
    
    // One-to-Many ilişkiler
    public ICollection<FlightTimesHistory> TimesHistory { get; set; } = new List<FlightTimesHistory>();
    public ICollection<Resource> Resources { get; set; } = new List<Resource>();
}
