using AODB.Application.Common.Interfaces;
using AODB.Domain.Entities;
using AODB.Domain.Enums;

namespace AODB.Application.Flights.Commands.UpdateFlightTimes;

public record UpdateFlightTimesCommand : IRequest
{
    public int FlightId { get; init; }
    public UpdateType UpdateType { get; init; } 
    public DateTime TimeValue { get; init; }
    public UpdateSource UpdateSource { get; init; }  
    public string? UpdateReason { get; init; }
    public string? UpdatedBy { get; init; }
}

public class UpdateFlightTimesCommandHandler : IRequestHandler<UpdateFlightTimesCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateFlightTimesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateFlightTimesCommand request, CancellationToken cancellationToken)
    {
        
        var flight = await _context.Flights
            .Include(f => f.CurrentTimes)
            .FirstOrDefaultAsync(f => f.Id == request.FlightId, cancellationToken);

        Guard.Against.NotFound(request.FlightId, flight);

       
        if (flight.CurrentTimes == null)
        {
            flight.CurrentTimes = new FlightTimesCurrent
            {
                FlightId = request.FlightId,
                LastUpdated = DateTime.UtcNow
            };
            _context.FlightTimesCurrent.Add(flight.CurrentTimes);
        }

        // ENUM TYPE
        switch (request.UpdateType)
        {
            case UpdateType.STA:
                flight.CurrentTimes.STA = request.TimeValue;
                break;
            case UpdateType.ETA:
                flight.CurrentTimes.ETA = request.TimeValue;
                break;
            case UpdateType.ATA:
                flight.CurrentTimes.ATA = request.TimeValue;
                break;
            case UpdateType.STD:
                flight.CurrentTimes.STD = request.TimeValue;
                break;
            case UpdateType.ETD:
                flight.CurrentTimes.ETD = request.TimeValue;
                break;
            case UpdateType.ATD:
                flight.CurrentTimes.ATD = request.TimeValue;
                break;
            case UpdateType.OffBlock:
                flight.CurrentTimes.OffBlockTime = request.TimeValue;
                break;
            case UpdateType.InBlock:
                flight.CurrentTimes.InBlockTime = request.TimeValue;
                break;
        }

        flight.CurrentTimes.LastUpdated = DateTime.UtcNow;

       
        var historyEntry = new FlightTimesHistory
        {
            FlightId = request.FlightId,
            UpdateType = request.UpdateType,  
            TimeValue = request.TimeValue,
            UpdateSource = request.UpdateSource,  
            UpdateReason = request.UpdateReason,
            InsertedAt = DateTime.UtcNow,
            InsertedBy = request.UpdatedBy
        };

        _context.FlightTimesHistory.Add(historyEntry);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
