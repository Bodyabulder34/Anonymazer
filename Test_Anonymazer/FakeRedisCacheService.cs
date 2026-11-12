using Anonymazer.Interfaces;
using StackExchange.Redis;

namespace Anonymazer.Tests;

public class FakeRedisCacheService : IRedisCacheService
{
    public bool ThrowOnGet { get; set; }
    public bool ThrowOnSet { get; set; }

    public Task<T?> GetAsync<T>(string key)
    {
        if (ThrowOnGet)
            throw new RedisConnectionException(
    ConnectionFailureType.UnableToConnect,
    "Redis is unavailable",
    null,
    CommandStatus.Unknown);

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null)
    {
        if (ThrowOnSet)
            throw new RedisConnectionException(
    ConnectionFailureType.UnableToConnect,
    "Redis is unavailable",
    null,
    CommandStatus.Unknown);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        return Task.CompletedTask;
    }
}