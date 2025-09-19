using AODB.Application.Common.Interfaces;

namespace AODB.Application.Airlines.Commands.DeleteAirline;

public record DeleteAirlineCommand(int Id) : IRequest;

public class DeleteAirlineCommandHandler : IRequestHandler<DeleteAirlineCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAirlineCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteAirlineCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Airlines
            .Where(l => l.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Airlines.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
