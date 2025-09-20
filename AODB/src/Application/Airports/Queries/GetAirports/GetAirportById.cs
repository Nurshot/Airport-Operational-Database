using System.Threading;
using AODB.Application.Airports.Queries.GetAirports;
using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;
using Microsoft.EntityFrameworkCore;
namespace AODB.Application.Airports.Queries.GetAirportById;

[Authorize]
public record GetAirportByIdQuery(int Id) : IRequest<AirportDto>;

public class GetAirportByIdQueryHandler : IRequestHandler<GetAirportByIdQuery, AirportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAirportByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<AirportDto> Handle(GetAirportByIdQuery request, CancellationToken cancellationToken)
    {
        var Airport = await _context.Airports
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .ProjectTo<AirportDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);  

        Guard.Against.NotFound(request.Id, Airport);  

        return Airport;
    }
}
