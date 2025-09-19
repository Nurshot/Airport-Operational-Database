using System.Text.Json.Serialization;

namespace AODB.Domain.Entities;
public class Airport : BaseAuditableEntity
{
    public string Name { get; set; } = null!;

    [JsonPropertyName("Iata_code")]
    public string Iata_code { get; set; } = null!;  //3 karakter

    [JsonPropertyName("Icao_code")]
    public string Icao_code { get; set; } = null!;  //4 karakter
    public string Country { get; set; } = null!;
    public string City { get; set; } = null!;

}


