namespace RideFlow.Core.Application.DTOs.Drivers;

public class DriverResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
    public bool IsActive { get; set; }
}