using AODB.Application.Common.Interfaces;
using FluentValidation;

namespace AODB.Application.Airports.Commands.UpdateAirport;

public class UpdateAirlineCommandValidator : AbstractValidator<UpdateAirportCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAirlineCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");


      
    }

    public async Task<bool> BeUniqueTitle(UpdateAirportCommand model, string name, CancellationToken cancellationToken)
    { 
        return await _context.Airports
            .Where(l => l.Id != model.Id)
            .AllAsync(l => l.Name != name, cancellationToken);
    }
}
