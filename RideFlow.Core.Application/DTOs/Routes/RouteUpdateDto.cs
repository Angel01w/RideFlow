namespace RideFlow.Core.Application.DTOs.Routes;

public class RouteUpdateDto
{
    public string Origin { get; set; } = null!;
    public string? Stops { get; set; }
    public string Destination { get; set; } = null!;
    public TimeSpan DepartureTime { get; set; }
    public bool IsActive { get; set; }
}