using Anonymazer.Services;

namespace Anonymazer.Tests;

public class AnonymizationFallbackTests
{
    [Fact]
    public async Task Anonymize_WhenRedisGetFails_StillReturnsResult()
    {
        var cache = new FakeRedisCacheService
        {
            ThrowOnGet = true
        };

        var service = CreateService(cache);

        var result = await service.AnonymizeWithStatisticsAsync(
            "Моя почта: ivan.petrov@example.com");

        Assert.Equal(
            "Моя почта: [EMAIL]",
            result.AnonymizedText);

        Assert.Equal(1, result.DetectedCounts["EMAIL"]);
    }

    [Fact]
    public async Task Anonymize_WhenRedisSetFails_StillReturnsResult()
    {
        var cache = new FakeRedisCacheService
        {
            ThrowOnSet = true
        };

        var service = CreateService(cache);

        var result = await service.AnonymizeWithStatisticsAsync(
            "Моя почта: ivan.petrov@example.com");

        Assert.Equal(
            "Моя почта: [EMAIL]",
            result.AnonymizedText);

        Assert.Equal(1, result.DetectedCounts["EMAIL"]);
    }

    private static AnonymizationService CreateService(
        FakeRedisCacheService cache)
    {
        return new AnonymizationService(
            new IDataDetector[]
            {
                new EmailDetector(),
                new PassportDetector(),
                new PhoneDetector()
            },
            cache);
    }
}