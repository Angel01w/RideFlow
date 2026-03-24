namespace RideFlow.Core.Domain.Entities;

public class Route
{
    public int Id { get; set; }
    public string Origin { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public TimeSpan DepartureTime { get; set; }
    public int DriverId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Driver Driver { get; set; } = null!;
    public ICollection<RouteAssignment> Assignments { get; set; } = new List<RouteAssignment>();
}