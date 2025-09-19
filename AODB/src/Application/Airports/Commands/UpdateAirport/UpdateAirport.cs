using System.Text.Json.Serialization;
using AODB.Application.Common.Interfaces;

namespace AODB.Application.Airports.Commands.UpdateAirport;

public record UpdateAirportCommand : IRequest
{

    public int Id { get; init; }
    public string? Name { get; init; }

    [JsonPropertyName("Iata_code")]
    public string? Iata_code { get; init; }   //3 karakter
    [JsonPropertyName("Icao_code")]
    public string? Icao_code { get; init; }   // 4 karakter

    public string? Country { get; init; }
    public string ?City { get; init; } 
}


public class UpdateAirportCommandHandler : IRequestHandler<UpdateAirportCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAirportCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAirportCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Airports
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            entity.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.Iata_code))
        {
            // Update tarafında command valid kaldırıldı. Buraya eklendi Çünkü boş null atarsak güncellemesin. Aksi halde command hata veriyor.
            if (request.Iata_code.Length == 3)
            {
                entity.Iata_code = request.Iata_code;
            }

        }

        if (!string.IsNullOrWhiteSpace(request.Icao_code))
        {
            if (request.Icao_code.Length == 4)
            {
                entity.Icao_code = request.Icao_code;
            }

        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            entity.Country = request.Country;
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            entity.City = request.City;
        }

        await _context.SaveChangesAsync(cancellationToken);

    }
}
