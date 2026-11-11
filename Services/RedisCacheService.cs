
using StackExchange.Redis;
using System.Text.Json;
using Anonymazer.Interfaces;
namespace Anonymazer.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task SetAsync<T>(
     string key,
     T value,
     TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value);

        if (expiration.HasValue)
        {
            await _database.StringSetAsync(
                key,
                json,
                expiration.Value);
        }
        else
        {
            await _database.StringSetAsync(
                key,
                json);
        }
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}
