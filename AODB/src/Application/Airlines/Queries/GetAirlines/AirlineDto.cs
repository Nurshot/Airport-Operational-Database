using System.Text.Json.Serialization;
using AODB.Domain.Entities;

namespace AODB.Application.Airlines.Queries.GetAirlines;

public class AircraftDto
{

    public int Id { get; init; }
    public string Name { get; init; } = null!;

    [JsonPropertyName("Iata_code")]
    public string Iata_code { get; init; } = null!;

    [JsonPropertyName("Icao_code")]
    public string Icao_code { get; init; } = null!;

    public string Country { get; init; } = null!;

    public string? LogoUrl { get; init; } 
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Airline, AircraftDto>();
        }
    }
}
