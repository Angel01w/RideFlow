using FluentValidation;
using RideFlow.Core.Application.DTOs.Drivers;

namespace RideFlow.Core.Application.Validators.Drivers;

public sealed class DriverCreateDtoValidator : AbstractValidator<DriverCreateDto>
{
    public DriverCreateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required")
            .MaximumLength(150);

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("LicenseNumber is required")
            .MaximumLength(50);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .MaximumLength(20);
    }
}