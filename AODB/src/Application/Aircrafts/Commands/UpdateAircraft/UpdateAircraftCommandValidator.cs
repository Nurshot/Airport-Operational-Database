using AODB.Application.Aircrafts.Commands.UpdateAircraft;
using AODB.Application.Common.Interfaces;
using FluentValidation;

namespace AODB.Application.Aircrafts.Commands.UpdateAircraft;

public class UpdateAircraftCommandValidator : AbstractValidator<UpdateAircraftCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAircraftCommandValidator(IApplicationDbContext context)
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
            .AllAsync(p=> p.Registration != registration, cancellationToken);
    }

    private async Task<bool> AirlineExists(int airlineId, CancellationToken cancellationToken)
    {
        return await _context.Airlines
            .AnyAsync(p => p.Id == airlineId, cancellationToken);
    }
}
