
using Anonymazer.Data;
using Anonymazer.Models;
using Microsoft.EntityFrameworkCore;

namespace Anonymazer.Services;

public class AnonymizationHistoryService
{
    private readonly AppDbContext _dbContext;

    public AnonymizationHistoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(AnonymizationResponse result, int userId)
    {
        var history = new AnonymizationHistory
        {
            OriginalText = result.OriginalText,
            AnonymizedText = result.AnonymizedText,
            CreatedAt = DateTime.UtcNow,
            DetectedCounts = new Dictionary<string, int>(result.DetectedCounts),
            UserId = userId
        };

        _dbContext.AnonymizationHistory.Add(history);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<AnonymizationHistoryPage> GetPageAsync(
        int userId,
        int page,
        int pageSize)
    {
        var query = _dbContext.AnonymizationHistory
            .AsNoTracking()
            .Where(history => history.UserId == userId)
            .OrderByDescending(history => history.CreatedAt)
            .ThenByDescending(history => history.Id);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new AnonymizationHistoryPage
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };
    }
}
