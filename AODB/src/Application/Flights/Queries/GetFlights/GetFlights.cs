using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;

namespace AODB.Application.Flights.Queries.GetFlights;

[Authorize]
public record GetFlightsQuery : IRequest<List<FlightDto>>;

public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, List<FlightDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFlightsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    //EN SON TEKRAR REVİZE EDİLECEK.
    public async Task<List<FlightDto>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Flights
            .Include(f => f.Airline)
            .Include(f => f.Aircraft)
            .Include(f => f.OriginAirport)
            .Include(f => f.DestinationAirport)
            .Include(f => f.CurrentTimes)
            .AsNoTracking()
            .ProjectTo<FlightDto>(_mapper.ConfigurationProvider)
            .OrderBy(f => f.FlightNumber)
            .ToListAsync(cancellationToken);
    }
}
