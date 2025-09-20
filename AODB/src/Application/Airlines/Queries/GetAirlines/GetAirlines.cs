using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;

namespace AODB.Application.Airlines.Queries.GetAirlines;

[Authorize]
public record GetAirlinesQuery : IRequest<List<AirlineDto>>;

public class GetAirlinesQueryHandler : IRequestHandler<GetAirlinesQuery, List<AirlineDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAirlinesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    //public async Task<AirportsVm> Handle(GetAirportsQuery request, CancellationToken cancellationToken)
    //{
    //    return new AirportsVm
    //    {
    //        Airports = await _context.Airports
    //            .AsNoTracking()
    //            .ProjectTo<AirportDto>(_mapper.ConfigurationProvider)
    //            .OrderBy(p => p.Id)
    //            .ToListAsync(cancellationToken)
    //    };
    //}
    public async Task<List<AirlineDto>> Handle(GetAirlinesQuery request, CancellationToken cancellationToken)
    {
        // Direkt liste
        return await _context.Airlines
            .AsNoTracking()
            .ProjectTo<AirlineDto>(_mapper.ConfigurationProvider)
            .OrderBy(p => p.Id)   
            .ToListAsync(cancellationToken);
    }
}
