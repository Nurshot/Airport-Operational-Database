namespace AODB.Domain.Enums;

public enum UpdateType
{
    STA = 1,    // Scheduled Time of Arrival
    ETA = 2,    // Estimated Time of Arrival
    ATA = 3,    // Actual Time of Arrival
    STD = 4,    // Scheduled Time of Departure
    ETD = 5,    // Estimated Time of Departure
    ATD = 6,    // Actual Time of Departure
    OffBlock = 7,
    InBlock = 8
}
