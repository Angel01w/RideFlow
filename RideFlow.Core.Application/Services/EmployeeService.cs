using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Application.DTOs.Employees;
using RideFlow.Core.Application.Interfaces;
using RideFlow.Core.Domain.Entities;
using RideFlow.Core.Persistence.Context;

namespace RideFlow.Core.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly RideFlowDbContext _context;

    public EmployeeService(RideFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeResponseDto>> GetAllAsync()
    {
        return await _context.Employees
            .Select(x => new EmployeeResponseDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Department = x.Department,
                Phone = x.Phone,
                Email = x.Email,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Where(x => x.Id == id)
            .Select(x => new EmployeeResponseDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Department = x.Department,
                Phone = x.Phone,
                Email = x.Email,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto)
    {
        var entity = new Employee
        {
            FullName = dto.FullName,
            Department = dto.Department,
            Phone = dto.Phone,
            Email = dto.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Employees.Add(entity);
        await _context.SaveChangesAsync();

        return new EmployeeResponseDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Department = entity.Department,
            Phone = entity.Phone,
            Email = entity.Email,
            IsActive = entity.IsActive
        };
    }

    public async Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        var entity = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return false;

        entity.FullName = dto.FullName;
        entity.Department = dto.Department;
        entity.Phone = dto.Phone;
        entity.Email = dto.Email;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return false;

        _context.Employees.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}