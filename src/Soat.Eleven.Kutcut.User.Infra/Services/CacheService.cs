using Microsoft.Extensions.Configuration;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Soat.Eleven.Kutcut.Users.Infra.Services;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConfiguration _configuration;
    public CacheService(IConfiguration configuration)
    {
        _configuration = configuration;

        var connectionString = configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis connection string is not configured.");
        _database = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
    }
    public async Task<T?> GetAsync<T>(string key)
    {
        string? cache = await _database.StringGetAsync(key);

        if (string.IsNullOrEmpty(cache))
            return default;
        
        return JsonSerializer.Deserialize<T>(cache);
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await _database.StringSetAsync(key, JsonSerializer.Serialize(value), TimeSpan.FromSeconds(_configuration.GetValue("RedisExpire", 10000)));
    }
}
