using AODB.Domain.Entities;
using AODB.Domain.Enums;

namespace AODB.Application.Flights.Queries.GetFlights;

public class FlightDto
{
    public int Id { get; init; }
    public string FlightNumber { get; init; } = null!;

    public int AirlineId { get; init; }
    public string AirlineName { get; init; } = null!;
    public string AirlineIataCode { get; init; } = null!;

    public int AircraftId { get; init; }
    public string AircraftRegistration { get; init; } = null!;
    public string AircraftType { get; init; } = null!;

    public int OriginAirportId { get; init; }
    public string OriginAirportName { get; init; } = null!;
    public string OriginAirportIataCode { get; init; } = null!;

    public int DestinationAirportId { get; init; }
    public string DestinationAirportName { get; init; } = null!;
    public string DestinationAirportIataCode { get; init; } = null!;

    public FlightType FlightType { get; init; }
    public FlightCategory Category { get; init; }
    public FlightStatus Status { get; init; }

    public DateTime? STA { get; init; }
    public DateTime? ETA { get; init; }
    public DateTime? ATA { get; init; }
    public DateTime? STD { get; init; }
    public DateTime? ETD { get; init; }
    public DateTime? ATD { get; init; }
    public DateTime? OffBlockTime { get; init; }
    public DateTime? InBlockTime { get; init; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Flight, FlightDto>()
                .ForMember(d => d.AirlineName, opt => opt.MapFrom(s => s.Airline.Name))
                .ForMember(d => d.AirlineIataCode, opt => opt.MapFrom(s => s.Airline.Iata_code))
                .ForMember(d => d.AircraftRegistration, opt => opt.MapFrom(s => s.Aircraft.Registration))
                .ForMember(d => d.AircraftType, opt => opt.MapFrom(s => s.Aircraft.Type))
                .ForMember(d => d.OriginAirportName, opt => opt.MapFrom(s => s.OriginAirport.Name))
                .ForMember(d => d.OriginAirportIataCode, opt => opt.MapFrom(s => s.OriginAirport.Iata_code))
                .ForMember(d => d.DestinationAirportName, opt => opt.MapFrom(s => s.DestinationAirport.Name))
                .ForMember(d => d.DestinationAirportIataCode, opt => opt.MapFrom(s => s.DestinationAirport.Iata_code))
                
                .ForMember(d => d.STA, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.STA : null))
                .ForMember(d => d.ETA, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.ETA : null))
                .ForMember(d => d.ATA, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.ATA : null))
                .ForMember(d => d.STD, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.STD : null))
                .ForMember(d => d.ETD, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.ETD : null))
                .ForMember(d => d.ATD, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.ATD : null))
                .ForMember(d => d.OffBlockTime, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.OffBlockTime : null))
                .ForMember(d => d.InBlockTime, opt => opt.MapFrom(s => s.CurrentTimes != null ? s.CurrentTimes.InBlockTime : null));
        }
    }
}
