namespace RideFlow.Core.Application.DTOs.Drivers;

public class DriverCreateDto
{
    public string FullName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string LicenseNumber { get; set; } = null!;
}