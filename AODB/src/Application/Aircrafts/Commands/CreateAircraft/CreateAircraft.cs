using System.Text.Json.Serialization;
using AODB.Application.Common.Interfaces;
using AODB.Domain.Entities;

namespace AODB.Application.Aircrafts.Commands.CreateAircraft;

public record CreateAircraftCommand : IRequest<int>
{

    public string Registration { get; init; } = null!;  // Benzersiz (unique), TC-JHK gibi
    public string Type { get; init; } = null!;          //Airbus A320, Boeing 737 gibi

    // İlişki -> Airline'a  Foreign Key
    public int AirlineId { get; init; }

}

public class CreateAircraftCommandHandler : IRequestHandler<CreateAircraftCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateAircraftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAircraftCommand request, CancellationToken cancellationToken)
    {

        var airlineExists = await _context.Airlines
            .AnyAsync(p => p.Id == request.AirlineId, cancellationToken);

        Guard.Against.NotFound(request.AirlineId, airlineExists);

        var entity = new Aircraft();

        entity.Registration = request.Registration;
        entity.Type = request.Type;
        entity.AirlineId = request.AirlineId;

        _context.Aircrafts.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
