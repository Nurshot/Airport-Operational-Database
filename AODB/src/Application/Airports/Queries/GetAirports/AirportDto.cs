using System.Text.Json.Serialization;
using AODB.Domain.Entities;

namespace AODB.Application.Airports.Queries.GetAirports;

public class AirportDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;

    [JsonPropertyName("Iata_code")]
    public string Iata_code { get; init; } = null!;

    [JsonPropertyName("Icao_code")]
    public string Icao_code { get; init; } = null!;

    public string City { get; init; } = null!;
    public string Country { get; init; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Airport, AirportDto>();
        }
    }
}
