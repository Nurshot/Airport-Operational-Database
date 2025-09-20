using AODB.Domain.Enums;

namespace AODB.Domain.Entities;

public class Resource : BaseAuditableEntity
{
    public int FlightId { get; set; }
    
    public string? Gate { get; set; }
    public string? Stand { get; set; }
    public string? BaggageBelt { get; set; }
    public string? CheckInDesk { get; set; }
    
    // Navigation Property
    public Flight Flight { get; set; } = null!;
}
