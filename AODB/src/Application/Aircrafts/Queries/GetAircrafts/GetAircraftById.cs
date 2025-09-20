using System.Threading;
using AODB.Application.Aircrafts.Queries.GetAircrafts;
using AODB.Application.Common.Interfaces;
using AODB.Application.Common.Security;
using Microsoft.EntityFrameworkCore;
namespace AODB.Application.Aircrafts.Queries.GetAircraftById;

[Authorize]
public record GetAircraftByIdQuery(int Id) : IRequest<AircraftDto>;

public class GetAircraftByIdQueryHandler : IRequestHandler<GetAircraftByIdQuery, AircraftDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAircraftByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<AircraftDto> Handle(GetAircraftByIdQuery request, CancellationToken cancellationToken)
    {
        var Aircraft = await _context.Aircrafts
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .ProjectTo<AircraftDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);  

        Guard.Against.NotFound(request.Id, Aircraft);  

        return Aircraft;
    }
}
