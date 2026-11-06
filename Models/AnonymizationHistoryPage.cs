using System.Collections.Generic;

namespace Anonymazer.Models;

public class AnonymizationHistoryPage
{
    public List<AnonymizationHistory> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}