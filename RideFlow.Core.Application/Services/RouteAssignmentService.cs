using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Application.DTOs.Assignments;
using RideFlow.Core.Application.Interfaces;
using RideFlow.Core.Domain.Entities;
using RideFlow.Core.Persistence.Context;

namespace RideFlow.Core.Application.Services;

public class RouteAssignmentService : IRouteAssignmentService
{
    private readonly RideFlowDbContext _context;

    public RouteAssignmentService(RideFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<RouteAssignmentResponseDto>> GetAllAsync()
    {
        return await _context.RouteAssignments
            .Where(x => x.IsActive)
            .Include(x => x.Employee)
            .Include(x => x.Route)
            .Select(x => new RouteAssignmentResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.FullName,
                RouteId = x.RouteId,
                RouteName = x.Route.Origin + " - " + x.Route.Destination,
                AssignedDate = x.AssignedDate,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<RouteAssignmentResponseDto?> GetByIdAsync(int id)
    {
        return await _context.RouteAssignments
            .Where(x => x.Id == id && x.IsActive)
            .Include(x => x.Employee)
            .Include(x => x.Route)
            .Select(x => new RouteAssignmentResponseDto
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                EmployeeName = x.Employee.FullName,
                RouteId = x.RouteId,
                RouteName = x.Route.Origin + " - " + x.Route.Destination,
                AssignedDate = x.AssignedDate,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RouteAssignmentResponseDto> CreateAsync(RouteAssignmentCreateDto dto)
    {
        var route = await _context.Routes.FirstOrDefaultAsync(x => x.Id == dto.RouteId && x.IsActive);
        if (route == null)
            throw new ArgumentException("Invalid or inactive route");

        var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == dto.EmployeeId && x.IsActive);
        if (employee == null)
            throw new ArgumentException("Invalid or inactive employee");

        var exists = await _context.RouteAssignments
            .AnyAsync(x => x.RouteId == dto.RouteId && x.EmployeeId == dto.EmployeeId && x.IsActive);

        if (exists)
            throw new ArgumentException("Employee already assigned to this route");

        var entity = new RouteAssignment
        {
            EmployeeId = dto.EmployeeId,
            RouteId = dto.RouteId,
            AssignedDate = DateTime.UtcNow,
            IsActive = true
        };

        _context.RouteAssignments.Add(entity);
        await _context.SaveChangesAsync();

        return new RouteAssignmentResponseDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeName = employee.FullName,
            RouteId = entity.RouteId,
            RouteName = route.Origin + " - " + route.Destination,
            AssignedDate = entity.AssignedDate,
            IsActive = entity.IsActive
        };
    }

    public async Task<bool> UpdateAsync(int id, RouteAssignmentUpdateDto dto)
    {
        var entity = await _context.RouteAssignments.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (entity == null)
            return false;

        var route = await _context.Routes.FirstOrDefaultAsync(x => x.Id == dto.RouteId && x.IsActive);
        if (route == null)
            throw new ArgumentException("Invalid or inactive route");

        var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Id == dto.EmployeeId && x.IsActive);
        if (employee == null)
            throw new ArgumentException("Invalid or inactive employee");

        var exists = await _context.RouteAssignments
            .AnyAsync(x =>
                x.Id != id &&
                x.RouteId == dto.RouteId &&
                x.EmployeeId == dto.EmployeeId &&
                x.IsActive);

        if (exists)
            throw new ArgumentException("Employee already assigned to this route");

        entity.EmployeeId = dto.EmployeeId;
        entity.RouteId = dto.RouteId;
        entity.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.RouteAssignments.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return false;

        _context.RouteAssignments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}