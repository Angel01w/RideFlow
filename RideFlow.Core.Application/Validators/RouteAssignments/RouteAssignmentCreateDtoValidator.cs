using FluentValidation;
using RideFlow.Core.Application.DTOs.Assignments;

namespace RideFlow.Core.Application.Validators.RouteAssignments;

public sealed class RouteAssignmentCreateDtoValidator : AbstractValidator<RouteAssignmentCreateDto>
{
    public RouteAssignmentCreateDtoValidator()
    {
        RuleFor(x => x.RouteId)
            .GreaterThan(0).WithMessage("RouteId must be greater than 0");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("EmployeeId must be greater than 0");
    }
}