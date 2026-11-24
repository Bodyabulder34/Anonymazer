using System.Collections.Generic;
using Anonymazer.Models;
using Anonymazer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Anonymazer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[EnableRateLimiting("ApiPolicy")]
public class AnonymizationController : ControllerBase
{
    private readonly AnonymizationService _anonymizationService;
    private readonly AnonymizationHistoryService _historyService;
    public AnonymizationController(
        AnonymizationService anonymizationService,
        AnonymizationHistoryService historyService)
    {
        _anonymizationService = anonymizationService;
        _historyService = historyService;
    }

    [HttpPost]
    public async Task<ActionResult<AnonymizationResponse>> Anonymize(
        [FromBody] AnonymizationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest(new
            {
                error = "Текст не должен быть пустым."
            });
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }


        var result = await _anonymizationService.AnonymizeWithStatisticsAsync(request.Text);
        await _historyService.SaveAsync(result, userId);

        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<ActionResult<AnonymizationHistoryPage>> GetHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                error = "Некорректные параметры пагинации."
            });
        }

        var history = await _historyService.GetPageAsync(
    userId,
    page,
    pageSize);

        return Ok(history);
    }

}
