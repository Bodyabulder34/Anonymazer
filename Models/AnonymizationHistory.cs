namespace Anonymazer.Models;

public class AnonymizationHistory
{
    public int Id { get; set; }

    public string OriginalText { get; set; } = string.Empty;

    public string AnonymizedText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int EmailsFound { get; set; }

    public int PhonesFound { get; set; }

    public int PassportsFound { get; set; }
}