namespace RideFlow.Core.Application.DTOs.Assignments;

public class RouteAssignmentUpdateDto
{
    public int EmployeeId { get; set; }
    public int RouteId { get; set; }
    public bool IsActive { get; set; }
}