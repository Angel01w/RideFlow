using RideFlow.Core.Application.DTOs.Employees;

namespace RideFlow.Core.Application.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeResponseDto>> GetAllAsync();
    Task<EmployeeResponseDto?> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto);
    Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}