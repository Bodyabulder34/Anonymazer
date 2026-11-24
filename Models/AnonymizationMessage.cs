namespace Anonymazer.Models;

public class AnonymizationMessage
{
    public Guid JobId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = string.Empty;
}