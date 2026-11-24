
using Anonymazer.Data;
using Anonymazer.Models;
using Microsoft.EntityFrameworkCore;

namespace Anonymazer.Services;

public class AdminStatisticsService
{
    private readonly AppDbContext _context;

    public AdminStatisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminStatisticsResponse> GetStatisticsAsync()
    {
        var history = await _context.AnonymizationHistory
            .AsNoTracking()
            .Select(item => new
            {
                item.UserId,
                item.CreatedAt,
                item.DetectedCounts
            })
            .ToListAsync();

        var usersCount = await _context.Users.CountAsync();

        var detectedCounts = new Dictionary<string, int>();

        foreach (var item in history)
        {
            foreach (var detection in item.DetectedCounts)
            {
                if (!detectedCounts.ContainsKey(detection.Key))
                    detectedCounts[detection.Key] = 0;

                detectedCounts[detection.Key] += detection.Value;
            }
        }

        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var dailyUsage = history
            .Where(item => item.CreatedAt >= thirtyDaysAgo)
            .GroupBy(item => DateOnly.FromDateTime(item.CreatedAt))
            .Select(group => new DailyUsageStatistic
            {
                Date = group.Key,
                Requests = group.Count()
            })
            .OrderBy(item => item.Date)
            .ToList();

        return new AdminStatisticsResponse
        {
            TotalRequests = history.Count,
            RegisteredUsers = usersCount,
            ActiveUsers = history
                .Where(item => item.UserId.HasValue)
                .Select(item => item.UserId)
                .Distinct()
                .Count(),
            DetectedDataCounts = detectedCounts,
            DailyUsage = dailyUsage
        };
    }
}
