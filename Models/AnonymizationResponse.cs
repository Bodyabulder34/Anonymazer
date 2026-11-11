
namespace Anonymazer.Models;

public class AnonymizationResponse
{
    public string OriginalText { get; set; } = string.Empty;

    public string AnonymizedText { get; set; } = string.Empty;

    public Dictionary<string, int> DetectedCounts { get; set; } = new();
}
