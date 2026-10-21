
using System.Linq;
using Anonymazer.Models;

namespace Anonymazer.Services;

public class AnonymizationService
{
    private readonly IDataDetector[] _detectors =
    {
        new EmailDetector(),
        new PassportDetector(),
        new PhoneDetector()
    };

    public AnonymizationResponse AnonymizeWithStatistics(string text)
    {
        text ??= string.Empty;

        var candidates = _detectors
            .SelectMany(detector => detector.Detect(text))
            .OrderBy(data => data.Start)
            .ThenByDescending(data => data.Length)
            .ThenBy(data => GetPriority(data.Type))
            .ToList();

        var selected = new List<DetectedData>();

        foreach (var candidate in candidates)
        {
            var overlappingIndex = selected.FindIndex(existing =>
                candidate.Start < existing.Start + existing.Length &&
                existing.Start < candidate.Start + candidate.Length);

            if (overlappingIndex == -1)
            {
                selected.Add(candidate);
                continue;
            }

            var existing = selected[overlappingIndex];

            if (GetPriority(candidate.Type) < GetPriority(existing.Type))
                selected[overlappingIndex] = candidate;
        }

        var response = new AnonymizationResponse
        {
            OriginalText = text,
            EmailsFound = selected.Count(data => data.Type == "EMAIL"),
            PhonesFound = selected.Count(data => data.Type == "PHONE"),
            PassportsFound = selected.Count(data => data.Type == "PASSPORT")
        };

        foreach (var item in selected.OrderByDescending(data => data.Start))
        {
            text = text.Remove(item.Start, item.Length)
                       .Insert(item.Start, $"[{item.Type}]");
        }

        response.AnonymizedText = text;
        return response;
    }

    public string Anonymize(string text)
    {
        return AnonymizeWithStatistics(text).AnonymizedText;
    }

    private static int GetPriority(string type)
    {
        return type switch
        {
            "PASSPORT" => 1,
            "EMAIL" => 2,
            "PHONE" => 3,
            _ => 4
        };
    }
}
