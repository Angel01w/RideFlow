using FluentValidation;
using RideFlow.Core.Application.DTOs.Attendance;

namespace RideFlow.Core.Application.Validators.Attendance;

public sealed class AttendanceCreateDtoValidator : AbstractValidator<AttendanceCreateDto>
{
    public AttendanceCreateDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("EmployeeId must be greater than 0");

        RuleFor(x => x.RouteId)
            .GreaterThan(0).WithMessage("RouteId must be greater than 0");

        RuleFor(x => x.AttendanceDate)
            .NotEmpty().WithMessage("AttendanceDate is required");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .MaximumLength(50);
    }
}