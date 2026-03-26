using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Application.DTOs.Routes;
using RideFlow.Core.Application.Interfaces;
using RideFlow.Core.Domain.Entities;
using RideFlow.Core.Persistence.Context;

namespace RideFlow.Core.Application.Services;

public class RouteService : IRouteService
{
    private readonly RideFlowDbContext _context;

    public RouteService(RideFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<RouteResponseDto>> GetAllAsync()
    {
        return await _context.Routes
            .Where(x => x.IsActive)
            .Select(x => new RouteResponseDto
            {
                Id = x.Id,
                Origin = x.Origin,
                Stops = x.Stops,
                Destination = x.Destination,
                DepartureTime = x.DepartureTime,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<RouteResponseDto?> GetByIdAsync(int id)
    {
        return await _context.Routes
            .Where(x => x.Id == id && x.IsActive)
            .Select(x => new RouteResponseDto
            {
                Id = x.Id,
                Origin = x.Origin,
                Stops = x.Stops,
                Destination = x.Destination,
                DepartureTime = x.DepartureTime,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<RouteResponseDto> CreateAsync(RouteCreateDto dto)
    {
        var entity = new Route
        {
            Origin = dto.Origin.Trim(),
            Stops = string.IsNullOrWhiteSpace(dto.Stops) ? null : dto.Stops.Trim(),
            Destination = dto.Destination.Trim(),
            DepartureTime = dto.DepartureTime,
            IsActive = true
        };

        _context.Routes.Add(entity);
        await _context.SaveChangesAsync();

        return new RouteResponseDto
        {
            Id = entity.Id,
            Origin = entity.Origin,
            Stops = entity.Stops,
            Destination = entity.Destination,
            DepartureTime = entity.DepartureTime,
            IsActive = entity.IsActive
        };
    }

    public async Task<bool> UpdateAsync(int id, RouteUpdateDto dto)
    {
        var entity = await _context.Routes.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (entity is null)
            return false;

        entity.Origin = dto.Origin.Trim();
        entity.Stops = string.IsNullOrWhiteSpace(dto.Stops) ? null : dto.Stops.Trim();
        entity.Destination = dto.Destination.Trim();
        entity.DepartureTime = dto.DepartureTime;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Routes.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        if (entity is null)
            return false;

        entity.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}