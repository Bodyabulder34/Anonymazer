using Anonymazer.Data;
using Anonymazer.Models;
using Anonymazer.Services;
using Microsoft.EntityFrameworkCore;

namespace Anonymazer.Tests;

public class AnonymizationHistoryServiceTests
{
    [Fact]
    public async Task GetPageAsync_ReturnsRequestedPage()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);

        context.AnonymizationHistory.AddRange(
            new AnonymizationHistory
            {
                OriginalText = "oldest",
                AnonymizedText = "oldest",
                CreatedAt = DateTime.UtcNow.AddMinutes(-3)
            },
            new AnonymizationHistory
            {
                OriginalText = "middle",
                AnonymizedText = "middle",
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new AnonymizationHistory
            {
                OriginalText = "newest",
                AnonymizedText = "newest",
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            }
        );

        await context.SaveChangesAsync();

        var service = new AnonymizationHistoryService(context);

        var result = await service.GetPageAsync(1, 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal("newest", result.Items[0].OriginalText);
        Assert.Equal("middle", result.Items[1].OriginalText);
    }
}