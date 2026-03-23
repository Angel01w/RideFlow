namespace RideFlow.Core.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}