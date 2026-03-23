using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Application.DTOs.Drivers;
using RideFlow.Core.Application.Interfaces;
using RideFlow.Core.Domain.Entities;
using RideFlow.Core.Persistence.Context;

namespace RideFlow.Core.Application.Services;

public class DriverService : IDriverService
{
    private readonly RideFlowDbContext _context;

    public DriverService(RideFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<DriverResponseDto>> GetAllAsync()
    {
        return await _context.Drivers
            .Select(x => new DriverResponseDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Phone = x.Phone,
                LicenseNumber = x.LicenseNumber,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<DriverResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Drivers
            .Where(x => x.Id == id)
            .Select(x => new DriverResponseDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Phone = x.Phone,
                LicenseNumber = x.LicenseNumber,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<DriverResponseDto> CreateAsync(DriverCreateDto dto)
    {
        var entity = new Driver
        {
            FullName = dto.FullName,
            Phone = dto.Phone,
            LicenseNumber = dto.LicenseNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Drivers.Add(entity);
        await _context.SaveChangesAsync();

        return new DriverResponseDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Phone = entity.Phone,
            LicenseNumber = entity.LicenseNumber,
            IsActive = entity.IsActive
        };
    }

    public async Task<bool> UpdateAsync(int id, DriverUpdateDto dto)
    {
        var entity = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return false;

        entity.FullName = dto.FullName;
        entity.Phone = dto.Phone;
        entity.LicenseNumber = dto.LicenseNumber;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return false;

        _context.Drivers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}