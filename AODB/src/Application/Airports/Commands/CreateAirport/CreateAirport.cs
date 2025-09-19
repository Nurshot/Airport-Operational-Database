using System.Text.Json.Serialization;
using AODB.Application.Common.Interfaces;
using AODB.Domain.Entities;

namespace AODB.Application.Airports.Commands.CreateAirport;

public record CreateAirportCommand : IRequest<int>
{
    public string Name { get; init; }= null!;

    [JsonPropertyName("Iata_code")]
    public string Iata_code { get; init; } = null!;    //3 karakter
    [JsonPropertyName("Icao_code")]
    public string Icao_code { get; init; } = null!;    // 4 karakter

    public string Country { get; init; } = null!;
    public string City { get; init; } = null!; 



}

public class CreateAirportCommandHandler : IRequestHandler<CreateAirportCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateAirportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAirportCommand request, CancellationToken cancellationToken)
    {
        var entity = new Airport();

        entity.Name = request.Name;
        entity.Iata_code = request.Iata_code;
        entity.Icao_code = request.Icao_code;
        entity.Country = request.Country;
        entity.City = request.City;

        _context.Airports.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
