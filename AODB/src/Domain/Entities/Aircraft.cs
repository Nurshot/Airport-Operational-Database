using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AODB.Domain.Entities;
public class Aircraft : BaseAuditableEntity
{
    public string Registration { get; set; } = null!;  // Benzersiz (unique), TC-JHK gibi
    public string Type { get; set; } = null!;          //Airbus A320, Boeing 737 gibi

    public int AirlineId { get; set; }
    //// İlişki -> Airline'a
    //[JsonIgnore]
    public Airline Airline { get; set; } = null!;

}
