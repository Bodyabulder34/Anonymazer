
using Anonymazer.Services;
using StackExchange.Redis;

namespace Anonymazer.Tests;

public class RedisCacheServiceTests
{
    private const string ConnectionString = "localhost:6379";

    [Fact]
    public async Task SetAsync_And_GetAsync_ReturnsSavedValue()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
        var cache = new RedisCacheService(redis);

        var key = $"test:{Guid.NewGuid()}";
        var expected = new TestData("Hello Redis", 123);

        try
        {
            await cache.SetAsync(key, expected, TimeSpan.FromMinutes(1));

            var actual = await cache.GetAsync<TestData>(key);

            Assert.NotNull(actual);
            Assert.Equal(expected.Message, actual.Message);
            Assert.Equal(expected.Number, actual.Number);
        }
        finally
        {
            await cache.RemoveAsync(key);
        }
    }

    [Fact]
    public async Task GetAsync_NonExistingKey_ReturnsDefault()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
        var cache = new RedisCacheService(redis);

        var key = $"test:missing:{Guid.NewGuid()}";

        var result = await cache.GetAsync<TestData>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_DeletesValue()
    {
        using var redis = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
        var cache = new RedisCacheService(redis);

        var key = $"test:remove:{Guid.NewGuid()}";

        await cache.SetAsync(key, new TestData("Delete me", 456));

        await cache.RemoveAsync(key);

        var result = await cache.GetAsync<TestData>(key);

        Assert.Null(result);
    }

    private record TestData(string Message, int Number);
}
