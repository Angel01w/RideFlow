namespace RideFlow.Core.Domain.Entities;

public class RouteAssignment
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int RouteId { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}