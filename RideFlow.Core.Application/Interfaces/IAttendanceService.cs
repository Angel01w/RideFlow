using RideFlow.Core.Application.DTOs.Attendance;

namespace RideFlow.Core.Application.Interfaces;

public interface IAttendanceService
{
    Task<List<AttendanceResponseDto>> GetAllAsync();
    Task<AttendanceResponseDto?> GetByIdAsync(int id);
    Task<AttendanceResponseDto> CreateAsync(AttendanceCreateDto dto);
    Task<bool> UpdateAsync(int id, AttendanceUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}