using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideFlow.Core.Application.Interfaces;

namespace RideFlow.Core.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("routes-usage")]
    public async Task<IActionResult> GetRouteUsage()
    {
        var result = await _service.GetRouteUsageAsync();
        return Ok(result);
    }

    [HttpGet("attendance-summary")]
    public async Task<IActionResult> GetAttendanceSummary()
    {
        var result = await _service.GetAttendanceSummaryAsync();
        return Ok(result);
    }

    [HttpGet("top-employees")]
    public async Task<IActionResult> GetTopEmployees()
    {
        var result = await _service.GetTopEmployeesAsync();
        return Ok(result);
    }
}