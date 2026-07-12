using GestaoProjetos.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoProjetos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _reportService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("completed")]
    public async Task<IActionResult> GetCompletedInPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var issues = await _reportService.GetCompletedInPeriodAsync(startDate, endDate);
        return Ok(issues);
    }

    [HttpGet("workload")]
    public async Task<IActionResult> GetWorkload()
    {
        var workload = await _reportService.GetInDevelopmentWorkloadAsync();
        return Ok(workload);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var pending = await _reportService.GetPendingIssuesAsync();
        return Ok(pending);
    }
}
