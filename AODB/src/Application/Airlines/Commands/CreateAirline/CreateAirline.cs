using System.Text.Json.Serialization;
using AODB.Application.Common.Interfaces;
using AODB.Domain.Entities;

namespace AODB.Application.Airlines.Commands.CreateAirline;

public record CreateAirlineCommand : IRequest<int>
{
    public string Name { get; init; }= null!;

    [JsonPropertyName("Iata_code")]
    public string Iata_code { get; init; } = null!;    //3 karakter

    [JsonPropertyName("Icao_code")]
    public string Icao_code { get; init; } = null!;    // 4 karakter
    public string Country { get; init; } = null!;
    public string? LogoUrl { get; init; }   //null olabilir şart değil

}

public class CreateAirlineCommandHandler : IRequestHandler<CreateAirlineCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateAirlineCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateAirlineCommand request, CancellationToken cancellationToken)
    {
        var entity = new Airline();

        entity.Name = request.Name;
        entity.Iata_code = request.Iata_code;
        entity.Icao_code = request.Icao_code;
        entity.Country = request.Country;
        entity.LogoUrl = request.LogoUrl;

        _context.Airlines.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
