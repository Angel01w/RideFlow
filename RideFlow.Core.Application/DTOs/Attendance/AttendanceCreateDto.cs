namespace RideFlow.Core.Application.DTOs.Attendance;

public class AttendanceCreateDto
{
    public int EmployeeId { get; set; }
    public int RouteId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public string Status { get; set; } = null!;
}