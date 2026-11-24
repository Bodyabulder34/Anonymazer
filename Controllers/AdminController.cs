
using Anonymazer.Models;
using Anonymazer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Anonymazer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[EnableRateLimiting("ApiPolicy")]
public class AdminController : ControllerBase
{
    private readonly AdminStatisticsService _statisticsService;

    public AdminController(AdminStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<AdminStatisticsResponse>> GetStatistics()
    {
        var statistics = await _statisticsService.GetStatisticsAsync();

        return Ok(statistics);
    }
}
