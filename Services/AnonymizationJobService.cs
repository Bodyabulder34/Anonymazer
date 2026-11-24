using Anonymazer.Data;
using Anonymazer.Models;
using Microsoft.EntityFrameworkCore;

namespace Anonymazer.Services;

public class AnonymizationJobService
{
    private readonly AppDbContext _context;

    public AnonymizationJobService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AnonymizationJob> CreateAsync(
        int userId,
        string text)
    {
        var job = new AnonymizationJob
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "Queued",
            OriginalText = text,
            CreatedAt = DateTime.UtcNow
        };

        _context.AnonymizationJobs.Add(job);

        await _context.SaveChangesAsync();

        return job;
    }

    public async Task<AnonymizationJob?> GetByIdAsync(
        Guid jobId,
        int userId)
    {
        return await _context.AnonymizationJobs
            .FirstOrDefaultAsync(job =>
                job.Id == jobId &&
                job.UserId == userId);
    }

    public async Task UpdateStatusAsync(
        Guid jobId,
        string status,
        string? anonymizedText = null,
        string? errorMessage = null)
    {
        var job = await _context.AnonymizationJobs
            .FirstOrDefaultAsync(job => job.Id == jobId);

        if (job == null)
            return;

        job.Status = status;
        job.AnonymizedText = anonymizedText;
        job.ErrorMessage = errorMessage;

        if (status == "Completed" || status == "Failed")
            job.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}