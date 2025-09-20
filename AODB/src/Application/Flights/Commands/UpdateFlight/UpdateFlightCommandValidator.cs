using AODB.Application.Common.Interfaces;

namespace AODB.Application.Flights.Commands.UpdateFlight;

public class UpdateFlightCommandValidator : AbstractValidator<UpdateFlightCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateFlightCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Flight ID is required.");

        RuleFor(v => v.FlightNumber)
            .NotEmpty().WithMessage("Flight number is required.")
            .MaximumLength(10).WithMessage("Flight number must not exceed 10 characters.")
            .MustAsync(BeUniqueFlightNumber)
            .WithMessage("The specified flight number already exists.");

        RuleFor(v => v.AirlineId)
            .NotEmpty().WithMessage("Airline is required.")
            .GreaterThan(0).WithMessage("Airline ID must be greater than 0.")
            .MustAsync(AirlineExists).WithMessage("Bu Airline bulunamadı");

        RuleFor(v => v.AircraftId)
            .NotEmpty().WithMessage("Aircraft is required.")
            .GreaterThan(0).WithMessage("Aircraft ID must be greater than 0.")
            .MustAsync(AircraftExists).WithMessage("Bu Aircraft bulunamadı");  

        RuleFor(v => v.OriginAirportId)
            .NotEmpty().WithMessage("Origin airport is required.")
            .GreaterThan(0).WithMessage("Origin airport ID must be greater than 0.")
            .MustAsync(AirportExists).WithMessage("Bu Airport bulunamadı");

        RuleFor(v => v.DestinationAirportId)
            .NotEmpty().WithMessage("Destination airport is required.")
            .GreaterThan(0).WithMessage("Destination airport ID must be greater than 0.")
            .MustAsync(AirportExists).WithMessage("Bu Airport bulunamadı")
            .NotEqual(p => p.OriginAirportId).WithMessage("Destination airport must be different from origin airport.");
    }

    public async Task<bool> BeUniqueFlightNumber(UpdateFlightCommand model, string flightNumber, CancellationToken cancellationToken)
    {
        return await _context.Flights
            .Where(f => f.Id != model.Id)
            .AllAsync(f => f.FlightNumber != flightNumber, cancellationToken);
    }

    private async Task<bool> AirlineExists(int airlineId, CancellationToken cancellationToken)
    {
        return await _context.Airlines
            .AnyAsync(a => a.Id == airlineId, cancellationToken);
    }

    private async Task<bool> AircraftExists(int aircraftId, CancellationToken cancellationToken)  // ✅ EKLE
    {
        return await _context.Aircrafts
            .AnyAsync(a => a.Id == aircraftId, cancellationToken);
    }

    private async Task<bool> AirportExists(int airportId, CancellationToken cancellationToken)
    {
        return await _context.Airports
            .AnyAsync(a => a.Id == airportId, cancellationToken);
    }
}
