
namespace Anonymazer.Models;

public class AnonymizationResponse
{
    public string OriginalText { get; set; } = string.Empty;
    public string AnonymizedText { get; set; } = string.Empty;

    public int EmailsFound { get; set; }
    public int PhonesFound { get; set; }
    public int PassportsFound { get; set; }
}
