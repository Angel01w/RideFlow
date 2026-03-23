namespace RideFlow.Core.Application.DTOs.Routes;

public class RouteCreateDto
{
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public TimeSpan DepartureTime { get; set; }
    public int DriverId { get; set; }
}