using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Application.DTOs.Attendance;
using RideFlow.Core.Application.Interfaces;
using RideFlow.Core.Domain.Entities;
using RideFlow.Core.Persistence.Context;

namespace RideFlow.Core.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly RideFlowDbContext _context;

    public AttendanceService(RideFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<AttendanceResponseDto>> GetAllAsync()
    {
        return await _context.Attendances
            .Select(x => new AttendanceResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                RouteId = x.RouteId,
                AttendanceDate = x.AttendanceDate,
                Status = x.Status,
                MarkedAt = x.MarkedAt
            })
            .ToListAsync();
    }

    public async Task<AttendanceResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Attendances
            .Where(x => x.Id == id)
            .Select(x => new AttendanceResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                RouteId = x.RouteId,
                AttendanceDate = x.AttendanceDate,
                Status = x.Status,
                MarkedAt = x.MarkedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AttendanceResponseDto> CreateAsync(AttendanceCreateDto dto)
    {
        var entity = new Attendance
        {
            EmployeeId = dto.EmployeeId,
            RouteId = dto.RouteId,
            AttendanceDate = dto.AttendanceDate,
            Status = dto.Status,
            MarkedAt = DateTime.UtcNow
        };

        _context.Attendances.Add(entity);
        await _context.SaveChangesAsync();

        return new AttendanceResponseDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            RouteId = entity.RouteId,
            AttendanceDate = entity.AttendanceDate,
            Status = entity.Status,
            MarkedAt = entity.MarkedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, AttendanceUpdateDto dto)
    {
        var entity = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return false;

        entity.EmployeeId = dto.EmployeeId;
        entity.RouteId = dto.RouteId;
        entity.AttendanceDate = dto.AttendanceDate;
        entity.Status = dto.Status;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Attendances.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return false;

        _context.Attendances.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}