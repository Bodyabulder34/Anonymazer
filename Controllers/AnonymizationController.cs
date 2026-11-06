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

        var result = _anonymizationService.AnonymizeWithStatistics(request.Text);

        await _historyService.SaveAsync(result);

        return Ok(result);
    }
}
