using FluentValidation;
using RideFlow.Core.Application.DTOs.Routes;

namespace RideFlow.Core.Application.Validators.Routes;

public sealed class RouteCreateDtoValidator : AbstractValidator<RouteCreateDto>
{
    public RouteCreateDtoValidator()
    {
        RuleFor(x => x.Origin)
            .NotEmpty().WithMessage("Origin is required")
            .MaximumLength(150);

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Destination is required")
            .MaximumLength(150);

        RuleFor(x => x.DepartureTime)
            .NotEmpty().WithMessage("DepartureTime is required");

        RuleFor(x => x.DriverId)
            .GreaterThan(0).WithMessage("DriverId must be greater than 0");
    }
}