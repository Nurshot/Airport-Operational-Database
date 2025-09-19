using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;

namespace AODB.Application.Airports.Queries.GetAirports;

[Authorize]
public record GetAirportsQuery : IRequest<List<AirportDto>>;

public class GetAirportsQueryHandler : IRequestHandler<GetAirportsQuery, List<AirportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAirportsQueryHandler(IApplicationDbContext context, IMapper mapper)
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
    public async Task<List<AirportDto>> Handle(GetAirportsQuery request, CancellationToken cancellationToken)
    {
        // Direkt liste
        return await _context.Airports
            .AsNoTracking()
            .ProjectTo<AirportDto>(_mapper.ConfigurationProvider)
            .OrderBy(p => p.Id)   //Belki sıralanmasına gerek yok performans için iptal edilebilir. Client tarafına yük verilir.
            .ToListAsync(cancellationToken);
    }
}
