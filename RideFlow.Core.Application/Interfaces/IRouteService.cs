using RideFlow.Core.Application.DTOs.Routes;

namespace RideFlow.Core.Application.Interfaces;

public interface IRouteService
{
    Task<List<RouteResponseDto>> GetAllAsync();
    Task<RouteResponseDto?> GetByIdAsync(int id);
    Task<RouteResponseDto> CreateAsync(RouteCreateDto dto);
    Task<bool> UpdateAsync(int id, RouteUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}