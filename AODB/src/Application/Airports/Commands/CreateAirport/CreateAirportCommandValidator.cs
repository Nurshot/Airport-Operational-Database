using AODB.Application.Common.Interfaces;

namespace AODB.Application.Airports.Commands.CreateAirport;

public class CreateAirportCommandValidator : AbstractValidator<CreateAirportCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateAirportCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(p=> p.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");

        RuleFor(p => p.Iata_code)
           .NotEmpty()
           .Length(3)
           .MustAsync(BeUniqueTitle)
               .WithMessage("'{PropertyName}' must be 3 Characters.")
               .WithErrorCode("Unique");

        RuleFor(p => p.Icao_code)
           .NotEmpty()
           .Length(4)
           .MustAsync(BeUniqueTitle)
               .WithMessage("'{PropertyName}' must be 4 Characters.")
               .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(string Name, CancellationToken cancellationToken)
    {
        return await _context.Airports
            .AllAsync(p => p.Name != Name, cancellationToken);
    }

    //public async Task<bool> BeLength(string Name, CancellationToken cancellationToken)
    //{
    //    if(Name.Length == 3)
    //        return await ;
    //    else if(Name.Length == 4)
    //        return await  4;
    //    else
    //        return await Name.Length == 3;
    //}
}
