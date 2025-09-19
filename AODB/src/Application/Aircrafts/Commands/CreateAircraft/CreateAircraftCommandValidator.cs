using AODB.Application.Aircrafts.Commands.CreateAircraft;
using AODB.Application.Common.Interfaces;

namespace AODB.Application.Airlines.Commands.CreateAircraft;

public class CreateAircraftCommandValidator : AbstractValidator<CreateAircraftCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateAircraftCommandValidator(IApplicationDbContext context)
    {
        _context = context;



        RuleFor(v => v.Registration)
           .NotEmpty().WithMessage("Registration gereklidir")
           .MaximumLength(200).WithMessage("Registration 200 karakterden uzun olamaz")
           .MustAsync(BeUniqueRegistration).WithMessage("Bu Registration zaten kullanılıyor");

        RuleFor(v => v.Type)
            .NotEmpty().WithMessage("Type gereklidir")
            .MaximumLength(100).WithMessage("Type 100 karakterden uzun olamaz");

        RuleFor(v => v.AirlineId)
            .GreaterThan(0).WithMessage("Geçerli bir AirlineId giriniz")
            .MustAsync(AirlineExists).WithMessage("Bu Airline bulunamadı");

    }

   private async Task<bool> BeUniqueRegistration(string registration, CancellationToken cancellationToken)
    {
        return await _context.Aircrafts
            .AllAsync(a => a.Registration != registration, cancellationToken);
    }

    private async Task<bool> AirlineExists(int airlineId, CancellationToken cancellationToken)
    {
        return await _context.Airlines
            .AnyAsync(a => a.Id == airlineId, cancellationToken);
    }

   
}
