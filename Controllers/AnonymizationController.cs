
using System.Security.Claims;
using Anonymazer.Models;
using Anonymazer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly AnonymizationJobService _jobService;
    private readonly RabbitMqProducer _rabbitMqProducer;

    public AnonymizationController(
        AnonymizationService anonymizationService,
        AnonymizationHistoryService historyService,
        AnonymizationJobService jobService,
        RabbitMqProducer rabbitMqProducer)
    {
        _anonymizationService = anonymizationService;
        _historyService = historyService;
        _jobService = jobService;
        _rabbitMqProducer = rabbitMqProducer;
    }

    // Синхронная анонимизация
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

        var result = await _anonymizationService
            .AnonymizeWithStatisticsAsync(request.Text);

        await _historyService.SaveAsync(result, userId);

        return Ok(result);
    }

    // Асинхронная анонимизация через RabbitMQ
    [HttpPost("async")]
    public async Task<IActionResult> AnonymizeAsync(
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

        var job = await _jobService.CreateAsync(userId, request.Text);

        var message = new AnonymizationMessage
        {
            JobId = job.Id,
            UserId = userId,
            Text = request.Text
        };

        await _rabbitMqProducer.PublishAsync(message);

        return Accepted(new
        {
            jobId = job.Id,
            status = job.Status,
            message = "Задание принято в обработку."
        });
    }

    [HttpGet("jobs/{jobId:guid}")]
    public async Task<IActionResult> GetJobStatus(Guid jobId)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var job = await _jobService.GetByIdAsync(jobId, userId);

        if (job == null)
        {
            return NotFound(new
            {
                error = "Задание не найдено."
            });
        }

        return Ok(new
        {
            job.Id,
            job.Status,
            job.CreatedAt,
            job.CompletedAt,
            job.AnonymizedText,
            job.ErrorMessage
        });
    }

    // История анонимизации пользователя
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
