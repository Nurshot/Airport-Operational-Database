using AODB.Domain.Enums;

namespace AODB.Domain.Entities;

public class FlightTimesHistory : BaseEntity
{
    public int FlightId { get; set; }
    
    public UpdateType UpdateType { get; set; }
    public DateTime TimeValue { get; set; }
    public UpdateSource UpdateSource { get; set; }
    public string? UpdateReason { get; set; }  // WX, MX, ATC restriction vs.
    
    public DateTime InsertedAt { get; set; } = DateTime.UtcNow;
    public string? InsertedBy { get; set; }
    
    // Navigation Property
    public Flight Flight { get; set; } = null!;
}
