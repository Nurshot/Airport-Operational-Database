using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;

namespace AODB.Application.Aircrafts.Queries.GetAircrafts;


[Authorize]
public record GetAircraftsQuery : IRequest<List<AircraftDto>>;

public class GetAircraftsQueryHandler : IRequestHandler<GetAircraftsQuery, List<AircraftDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAircraftsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AircraftDto>> Handle(GetAircraftsQuery request, CancellationToken cancellationToken)
    {
        // Direkt liste
        return await _context.Aircrafts
            .AsNoTracking()
            .ProjectTo<AircraftDto>(_mapper.ConfigurationProvider)
            .OrderBy(p => p.Id)   
            .ToListAsync(cancellationToken);
    }
}
