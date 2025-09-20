using System.Threading;
using AODB.Application.Airlines.Queries.GetAirlines;
using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;
using Microsoft.EntityFrameworkCore;
namespace AODB.Application.Airlines.Queries.GetAirlineById;

[Authorize]
public record GetAirlineByIdQuery(int Id) : IRequest<AirlineDto>;

public class GetAirlineByIdQueryHandler : IRequestHandler<GetAirlineByIdQuery, AirlineDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAirlineByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<AirlineDto> Handle(GetAirlineByIdQuery request, CancellationToken cancellationToken)
    {
        var Airline = await _context.Airlines
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .ProjectTo<AirlineDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);  

        Guard.Against.NotFound(request.Id, Airline);  

        return Airline;
    }
}
