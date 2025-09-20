using AODB.Application.Common.Interfaces;
using AODB.Domain.Entities;
using AODB.Domain.Enums;

namespace AODB.Application.Flights.Commands.CreateFlight;

public record CreateFlightCommand : IRequest<int>
{
    public string FlightNumber { get; init; } = null!;
    public int AirlineId { get; init; }
    public int AircraftId { get; init; }
    public int OriginAirportId { get; init; }
    public int DestinationAirportId { get; init; }
    public FlightType FlightType { get; init; }
    public FlightCategory Category { get; init; }
    public FlightStatus Status { get; init; } = FlightStatus.Scheduled;
}

public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
    {
        
        var airlineExists = await _context.Airlines
            .AnyAsync(a => a.Id == request.AirlineId, cancellationToken);
        Guard.Against.NotFound(request.AirlineId, airlineExists);
        
        var aircraftExists = await _context.Aircrafts
            .AnyAsync(a => a.Id == request.AircraftId, cancellationToken);
        Guard.Against.NotFound(request.AircraftId, aircraftExists);
        
        var originExists = await _context.Airports
            .AnyAsync(a => a.Id == request.OriginAirportId, cancellationToken);
        Guard.Against.NotFound(request.OriginAirportId, originExists);
        
        var destinationExists = await _context.Airports
            .AnyAsync(a => a.Id == request.DestinationAirportId, cancellationToken);
        Guard.Against.NotFound(request.DestinationAirportId, destinationExists);

        var entity = new Flight
        {
            FlightNumber = request.FlightNumber,
            AirlineId = request.AirlineId,
            AircraftId = request.AircraftId,
            OriginAirportId = request.OriginAirportId,
            DestinationAirportId = request.DestinationAirportId,
            FlightType = request.FlightType,
            Category = request.Category,
            Status = request.Status
        };

        _context.Flights.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
