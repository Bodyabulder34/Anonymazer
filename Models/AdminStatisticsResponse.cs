
namespace Anonymazer.Models;

public class AdminStatisticsResponse
{
    public int TotalRequests { get; set; }

    public int RegisteredUsers { get; set; }

    public int ActiveUsers { get; set; }

    public Dictionary<string, int> DetectedDataCounts { get; set; } = new();

    public List<DailyUsageStatistic> DailyUsage { get; set; } = new();
}

public class DailyUsageStatistic
{
    public DateOnly Date { get; set; }

    public int Requests { get; set; }
}
