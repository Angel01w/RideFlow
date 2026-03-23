using RideFlow.Core.Application.DTOs.Reports;

namespace RideFlow.Core.Application.Interfaces;

public interface IReportService
{
    Task<List<RouteUsageReportDto>> GetRouteUsageAsync();
    Task<List<AttendanceSummaryDto>> GetAttendanceSummaryAsync();
    Task<List<TopEmployeesDto>> GetTopEmployeesAsync();
}