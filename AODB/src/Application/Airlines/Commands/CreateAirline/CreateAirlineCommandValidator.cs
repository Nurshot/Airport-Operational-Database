using AODB.Application.Common.Interfaces;

namespace AODB.Application.Airlines.Commands.CreateAirline;

public class CreateAircraftCommandValidator : AbstractValidator<CreateAirlineCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateAircraftCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");


        RuleFor(v => v.Iata_code)
            .NotEmpty()
           .Length(3)
               .WithMessage("'{PropertyName}' must be 3 Characters.")
               .WithErrorCode("Unique");

        RuleFor(v => v.Icao_code)
            .NotEmpty()
           .Length(4)
               .WithMessage("'{PropertyName}' must be 4 Characters.")
               .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(string Name, CancellationToken cancellationToken)
    {
        return await _context.Airlines
            .AllAsync(l => l.Name != Name, cancellationToken);
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
