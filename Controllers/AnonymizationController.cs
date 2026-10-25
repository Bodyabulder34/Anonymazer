using Anonymazer.Data;
using Anonymazer.Models;
using Anonymazer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anonymazer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnonymizationController : ControllerBase
{
    private readonly AnonymizationService _anonymizationService;
    private readonly AppDbContext _dbContext;

    public AnonymizationController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _anonymizationService = new AnonymizationService();
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

        var history = new AnonymizationHistory
        {
            OriginalText = result.OriginalText,
            AnonymizedText = result.AnonymizedText,
            CreatedAt = DateTime.UtcNow,
            EmailsFound = result.EmailsFound,
            PhonesFound = result.PhonesFound,
            PassportsFound = result.PassportsFound
        };

        _dbContext.AnonymizationHistory.Add(history);
        await _dbContext.SaveChangesAsync();

        return Ok(result);
    }
}