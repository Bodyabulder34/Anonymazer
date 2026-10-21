
using Anonymazer.Models;
using Anonymazer.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anonymazer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnonymizationController : ControllerBase
{
    private readonly AnonymizationService _anonymizationService;

    public AnonymizationController()
    {
        _anonymizationService = new AnonymizationService();
    }

    [HttpPost]
    public ActionResult<AnonymizationResponse> Anonymize(
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

        return Ok(result);
    }
}
