
namespace Anonymazer.Models;

public class DetectedData
{
    public int Start { get; set; }

    public int Length { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}
