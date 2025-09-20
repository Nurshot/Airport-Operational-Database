using AODB.Domain.Enums;

namespace AODB.Domain.Entities;

public class FlightTimesCurrent : BaseAuditableEntity
{
    // Primary Key = FlightId (One-to-One ilişki)
    public int FlightId { get; set; }
    
    // Arrival Times
    public DateTime? STA { get; set; }  // Scheduled Time of Arrival
    public DateTime? ETA { get; set; }  // Estimated Time of Arrival
    public DateTime? ATA { get; set; }  // Actual Time of Arrival
    
    // Departure Times
    public DateTime? STD { get; set; }  // Scheduled Time of Departure
    public DateTime? ETD { get; set; }  // Estimated Time of Departure
    public DateTime? ATD { get; set; }  // Actual Time of Departure
    
    // Block Times
    public DateTime? OffBlockTime { get; set; }
    public DateTime? InBlockTime { get; set; }
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    
    // Navigation Property
    public Flight Flight { get; set; } = null!;
}
