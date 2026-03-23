using RideFlow.Core.Application.DTOs.Drivers;

namespace RideFlow.Core.Application.Interfaces;

public interface IDriverService
{
    Task<List<DriverResponseDto>> GetAllAsync();
    Task<DriverResponseDto?> GetByIdAsync(int id);
    Task<DriverResponseDto> CreateAsync(DriverCreateDto dto);
    Task<bool> UpdateAsync(int id, DriverUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}