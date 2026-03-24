using FluentValidation;
using RideFlow.Core.Application.DTOs.Employees;

namespace RideFlow.Core.Application.Validators.Employees;

public sealed class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
{
    public EmployeeCreateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required")
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress();

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required")
            .MaximumLength(20);
    }
}