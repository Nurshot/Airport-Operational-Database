using AODB.Application.Common.Interfaces;
using AODB.Domain.Enums;

namespace AODB.Application.Flights.Commands.UpdateFlight;

public record UpdateFlightCommand : IRequest
{
    public int Id { get; init; }
    public string FlightNumber { get; init; } = null!;
    public int AirlineId { get; init; }
    public int AircraftId { get; init; }
    public int OriginAirportId { get; init; }
    public int DestinationAirportId { get; init; }
    public FlightType FlightType { get; init; }
    public FlightCategory Category { get; init; }
    public FlightStatus Status { get; init; }
}

public class UpdateFlightCommandHandler : IRequestHandler<UpdateFlightCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateFlightCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Flights
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

       
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

        entity.FlightNumber = request.FlightNumber;
        entity.AirlineId = request.AirlineId;
        entity.AircraftId = request.AircraftId;
        entity.OriginAirportId = request.OriginAirportId;
        entity.DestinationAirportId = request.DestinationAirportId;
        entity.FlightType = request.FlightType;
        entity.Category = request.Category;
        entity.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
