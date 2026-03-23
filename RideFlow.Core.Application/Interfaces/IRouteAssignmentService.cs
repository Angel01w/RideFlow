using RideFlow.Core.Application.DTOs.Assignments;

namespace RideFlow.Core.Application.Interfaces;

public interface IRouteAssignmentService
{
    Task<List<RouteAssignmentResponseDto>> GetAllAsync();
    Task<RouteAssignmentResponseDto?> GetByIdAsync(int id);
    Task<RouteAssignmentResponseDto> CreateAsync(RouteAssignmentCreateDto dto);
    Task<bool> UpdateAsync(int id, RouteAssignmentUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}