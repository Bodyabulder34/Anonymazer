
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

        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "test-hash"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        context.AnonymizationHistory.AddRange(
            new AnonymizationHistory
            {
                UserId = user.Id,
                OriginalText = "oldest",
                AnonymizedText = "oldest",
                CreatedAt = DateTime.UtcNow.AddMinutes(-3)
            },
            new AnonymizationHistory
            {
                UserId = user.Id,
                OriginalText = "middle",
                AnonymizedText = "middle",
                CreatedAt = DateTime.UtcNow.AddMinutes(-2)
            },
            new AnonymizationHistory
            {
                UserId = user.Id,
                OriginalText = "newest",
                AnonymizedText = "newest",
                CreatedAt = DateTime.UtcNow.AddMinutes(-1)
            }
        );

        await context.SaveChangesAsync();

        var service = new AnonymizationHistoryService(context);

        var result = await service.GetPageAsync(user.Id, 1, 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal("newest", result.Items[0].OriginalText);
        Assert.Equal("middle", result.Items[1].OriginalText);
    }
}
