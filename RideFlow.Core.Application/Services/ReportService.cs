using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Application.DTOs.Reports;
using RideFlow.Core.Application.Interfaces;
using RideFlow.Core.Persistence.Context;

namespace RideFlow.Core.Application.Services;

public class ReportService : IReportService
{
    private readonly RideFlowDbContext _context;

    public ReportService(RideFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<RouteUsageReportDto>> GetRouteUsageAsync()
    {
        return await _context.RouteAssignments
            .Where(x => x.IsActive)
            .GroupBy(x => x.RouteId)
            .Select(g => new RouteUsageReportDto
            {
                RouteId = g.Key,
                TotalEmployees = g.Count()
            })
            .ToListAsync();
    }

    public async Task<List<AttendanceSummaryDto>> GetAttendanceSummaryAsync()
    {
        return await _context.Attendances
            .GroupBy(x => x.AttendanceDate.Date)
            .Select(g => new AttendanceSummaryDto
            {
                Date = g.Key,
                TotalPresent = g.Count(x => x.Status == "Present"),
                TotalAbsent = g.Count(x => x.Status == "Absent")
            })
            .ToListAsync();
    }

    public async Task<List<TopEmployeesDto>> GetTopEmployeesAsync()
    {
        return await _context.Attendances
            .Where(x => x.Status == "Present")
            .GroupBy(x => x.EmployeeId)
            .Select(g => new TopEmployeesDto
            {
                EmployeeId = g.Key,
                TotalAttendances = g.Count()
            })
            .OrderByDescending(x => x.TotalAttendances)
            .ToListAsync();
    }
}