namespace RideFlow.Core.Application.DTOs.Attendance;

public class AttendanceResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int RouteId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public string Status { get; set; } = null!;
    public DateTime MarkedAt { get; set; }
}