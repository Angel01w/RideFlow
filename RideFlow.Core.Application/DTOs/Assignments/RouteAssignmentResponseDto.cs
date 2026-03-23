namespace RideFlow.Core.Application.DTOs.Assignments;

public class RouteAssignmentResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int RouteId { get; set; }
    public DateTime AssignedDate { get; set; }
    public bool IsActive { get; set; }
}