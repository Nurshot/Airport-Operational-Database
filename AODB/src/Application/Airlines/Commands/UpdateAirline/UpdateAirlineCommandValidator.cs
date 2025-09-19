using AODB.Application.Common.Interfaces;
using FluentValidation;

namespace AODB.Application.Airlines.Commands.UpdateAirline;

public class UpdateAircraftCommandValidator : AbstractValidator<UpdateAirlineCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAircraftCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");

       
    }

    public async Task<bool> BeUniqueTitle(UpdateAirlineCommand model, string name, CancellationToken cancellationToken)
    { 
        return await _context.Airlines
            .Where(l => l.Id != model.Id)
            .AllAsync(l => l.Name != name, cancellationToken);
    }
}
