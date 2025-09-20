using AODB.Application.Common.Interfaces;

namespace AODB.Application.Flights.Commands.DeleteFlight;

public record DeleteFlightCommand(int Id) : IRequest;

public class DeleteFlightCommandHandler : IRequestHandler<DeleteFlightCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteFlightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteFlightCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Flights
            .Include(f => f.CurrentTimes)
            .Include(f => f.TimesHistory)
            .Include(f => f.Resources)
            .Where(f => f.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        
        if (entity.CurrentTimes != null)
        {
            _context.FlightTimesCurrent.Remove(entity.CurrentTimes);
        }

        if (entity.TimesHistory.Any())
        {
            _context.FlightTimesHistory.RemoveRange(entity.TimesHistory);
        }

        if (entity.Resources.Any())
        {
            _context.Resources.RemoveRange(entity.Resources);
        }

        _context.Flights.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
