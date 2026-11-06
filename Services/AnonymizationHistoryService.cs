using Anonymazer.Data;
using Anonymazer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
namespace Anonymazer.Services;

public class AnonymizationHistoryService
{
    private readonly AppDbContext _dbContext;

    public AnonymizationHistoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(AnonymizationResponse result)
    {
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
    }
    public async Task<List<AnonymizationHistory>> GetAllAsync()
    {
        return await _dbContext.AnonymizationHistory
            .AsNoTracking()
            .OrderByDescending(history => history.CreatedAt)
            .ToListAsync();
    }

}