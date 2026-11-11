
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class AnonymizationService
{
    private readonly IDataDetector[] _detectors;
    private readonly RedisCacheService _cacheService;

    public AnonymizationService(
        IEnumerable<IDataDetector> detectors,
        RedisCacheService cacheService)
    {
        _detectors = detectors.ToArray();
        _cacheService = cacheService;
    }

    public AnonymizationService(IEnumerable<IDataDetector> detectors)
    {
        _detectors = detectors.ToArray();
        _cacheService = null!;
    }

    public async Task<AnonymizationResponse> AnonymizeWithStatisticsAsync(string text)
    {
        text ??= string.Empty;

        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(text)));

        var cacheKey = $"anonymizer:result:{hash}";

        var cachedResult =
            await _cacheService.GetAsync<AnonymizationResponse>(cacheKey);

        if (cachedResult != null)
            return cachedResult;

        var result = AnonymizeWithStatistics(text);

        await _cacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(10));

        return result;
    }

    public AnonymizationResponse AnonymizeWithStatistics(string text)
    {
        text ??= string.Empty;

        var candidates = _detectors
            .SelectMany(detector => detector.Detect(text))
            .Where(data =>
                data.Start >= 0 &&
                data.Length > 0 &&
                data.Start <= text.Length &&
                data.Length <= text.Length - data.Start)
            .OrderBy(data => data.Priority)
            .ThenBy(data => data.Start)
            .ThenByDescending(data => data.Length)
            .ToList();

        var selected = new List<DetectedData>();

        foreach (var candidate in candidates)
        {
            var overlaps = selected.Any(existing =>
                candidate.Start < existing.Start + existing.Length &&
                existing.Start < candidate.Start + candidate.Length);

            if (!overlaps)
                selected.Add(candidate);
        }

        var response = new AnonymizationResponse
        {
            OriginalText = text,
            DetectedCounts = selected
                .GroupBy(data => data.Type)
                .ToDictionary(group => group.Key, group => group.Count())
        };

        foreach (var item in selected.OrderByDescending(data => data.Start))
        {
            var replacement = string.IsNullOrEmpty(item.Replacement)
                ? $"[{item.Type}]"
                : item.Replacement;

            text = text.Remove(item.Start, item.Length)
                       .Insert(item.Start, replacement);
        }

        response.AnonymizedText = text;

        return response;
    }

    public string Anonymize(string text)
    {
        return AnonymizeWithStatistics(text).AnonymizedText;
    }
}
