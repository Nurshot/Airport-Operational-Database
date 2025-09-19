using System.Text.Json.Serialization;
using AODB.Application.Common.Interfaces;

namespace AODB.Application.Airlines.Commands.UpdateAirline;

public record UpdateAirlineCommand : IRequest
{

    public int Id { get; init; }
    public string? Name { get; init; }

    [JsonPropertyName("Iata_code")]
    public string? Iata_code { get; init; }   //3 karakter
    [JsonPropertyName("Icao_code")]
    public string? Icao_code { get; init; }   // 4 karakter

    public string? Country { get; init; }
    public string ? LogoUrl { get; init; } 
}


public class UpdateAirlineCommandHandler : IRequestHandler<UpdateAirlineCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAirlineCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateAirlineCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Airlines
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            entity.Name = request.Name;
        }

        if (!string.IsNullOrWhiteSpace(request.Iata_code))
        {
            // Update tarafında command valid kaldırıldı. Buraya eklendi Çünkü boş null atarsak güncellemesin. Aksi halde command hata veriyor.
            if(request.Iata_code.Length == 3)
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

        if (!string.IsNullOrWhiteSpace(request.LogoUrl))
        {
            entity.LogoUrl = request.LogoUrl;
        }

        await _context.SaveChangesAsync(cancellationToken);

    }
}
