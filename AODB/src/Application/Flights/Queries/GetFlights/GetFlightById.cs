using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;
using AODB.Application.Flights.Queries.GetFlights;
namespace AODB.Application.Flights.Queries.GetFlightById;

[Authorize]
public record GetFlightByIdQuery(int Id) : IRequest<FlightDto>;

public class GetFlightByIdQueryHandler : IRequestHandler<GetFlightByIdQuery, FlightDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFlightByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    public async Task<FlightDto> Handle(GetFlightByIdQuery request, CancellationToken cancellationToken)
    {
        var flight = await _context.Flights
           .Include(f => f.Airline)
           .Include(f => f.Aircraft)
           .Include(f => f.OriginAirport)
           .Include(f => f.DestinationAirport)
           .Include(f => f.CurrentTimes)
           .AsNoTracking()
           .Where(f => f.Id == request.Id)  
           .ProjectTo<FlightDto>(_mapper.ConfigurationProvider)
           .FirstOrDefaultAsync(cancellationToken);  

        Guard.Against.NotFound(request.Id, flight);  

        return flight;
    }
}
