using System.Text.Json.Serialization;
using AODB.Domain.Entities;

namespace AODB.Application.Aircrafts.Queries.GetAircrafts;

public class AircraftDto
{
    public int Id { get; init; }
    public string Registration { get; init; } = null!;  // (unique)
    public string Type { get; init; } = null!;          //Airbus A320, Boeing 737 gibi

    //Airline'a  Foreign Key
    public int AirlineId { get; init; }
    public Airline Airline { get; init; } = null!;


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Aircraft, AircraftDto>();
        }
    }
}
