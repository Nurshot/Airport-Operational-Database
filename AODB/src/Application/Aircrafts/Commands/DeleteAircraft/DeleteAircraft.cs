using AODB.Application.Common.Interfaces;

namespace AODB.Application.Aircrafts.Commands.DeleteAircraft;

public record DeleteAircraftCommand(int Id) : IRequest;

public class DeleteAircraftCommandHandler : IRequestHandler<DeleteAircraftCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAircraftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteAircraftCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Aircrafts
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Aircrafts.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
