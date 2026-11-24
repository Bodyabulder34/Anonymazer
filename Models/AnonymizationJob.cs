namespace Anonymazer.Models;

public class AnonymizationJob
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = "Queued";

    public string OriginalText { get; set; } = string.Empty;

    public string? AnonymizedText { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public string? ErrorMessage { get; set; }
}