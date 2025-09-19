using AODB.Application.Common.Interfaces;

namespace AODB.Application.Airports.Commands.DeleteAirport;

public record DeleteAirportCommand(int Id) : IRequest;

public class DeleteAirportCommandHandler : IRequestHandler<DeleteAirportCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAirportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteAirportCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Airports
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Airports.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
