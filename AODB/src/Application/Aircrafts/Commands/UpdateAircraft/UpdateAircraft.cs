using System.Text.Json.Serialization;
using AODB.Application.Common.Interfaces;
using AODB.Domain.Entities;

namespace AODB.Application.Aircrafts.Commands.UpdateAircraft;

public record UpdateAircraftCommand : IRequest
{

    public int Id { get; init; }
    public string Registration { get; init; } = null!;  // Benzersiz (unique), TC-JHK gibi
    public string Type { get; init; } = null!;          //Airbus A320, Boeing 737 gibi

    // İlişki -> Airline'a  Foreign Key
    public int AirlineId { get; init; }


}


public class UpdateAircraftCommandHandler : IRequestHandler<UpdateAircraftCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAircraftCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAircraftCommand request, CancellationToken cancellationToken)
    {
        //Nolur nolmaz burda bida validation yapılıyor.

        var entity = await _context.Aircrafts
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        var airlineexist = await _context.Airlines
           .FindAsync(new object[] { request.AirlineId }, cancellationToken);

        Guard.Against.NotFound(request.AirlineId, airlineexist);


        if (!string.IsNullOrWhiteSpace(request.Registration))
        {
            entity.Registration = request.Registration;
        }

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
           
             entity.Type = request.Type;
           
        }


        entity.AirlineId = request.AirlineId;
        

        
       
        
        await _context.SaveChangesAsync(cancellationToken);

    }
}
