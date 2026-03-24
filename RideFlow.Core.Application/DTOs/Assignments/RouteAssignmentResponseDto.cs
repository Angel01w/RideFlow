namespace RideFlow.Core.Application.DTOs.Assignments;

public class RouteAssignmentResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public int RouteId { get; set; }
    public string RouteName { get; set; } = null!;
    public DateTime AssignedDate { get; set; }
    public bool IsActive { get; set; }
}