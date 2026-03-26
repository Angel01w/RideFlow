namespace RideFlow.Core.Domain.Entities;

public class Route
{
    public int Id { get; set; }
    public string Origin { get; set; } = null!;
    public string? Stops { get; set; }
    public string Destination { get; set; } = null!;
    public TimeSpan DepartureTime { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<RouteAssignment> Assignments { get; set; } = new List<RouteAssignment>();
}