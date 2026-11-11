using System.Collections.Generic;
using Anonymazer.Models;
using Anonymazer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anonymazer.Controllers;

[ApiController]
[Route("api/[controller]")]
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



        var result = await _anonymizationService.AnonymizeWithStatisticsAsync(request.Text);
        await _historyService.SaveAsync(result);

        return Ok(result);
    }

    [HttpGet("history")]
    public async Task<ActionResult<AnonymizationHistoryPage>> GetHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                error = "Некорректные параметры пагинации."
            });
        }

        var history = await _historyService.GetPageAsync(page, pageSize);

        return Ok(history);
    }

}
