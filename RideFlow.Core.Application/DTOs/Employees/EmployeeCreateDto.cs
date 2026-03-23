namespace RideFlow.Core.Application.DTOs.Employees;

public class EmployeeCreateDto
{
    public string FullName { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
}