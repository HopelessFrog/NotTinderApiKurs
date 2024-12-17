using Newtonsoft.Json;
using StackExchange.Redis;

namespace Redis.Cache;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly int _expire = 60;
    private readonly IConnectionMultiplexer _redis;


    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = _redis.GetDatabase();
    }


    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var data = await _database.StringGetAsync(key);

        if (data.IsNullOrEmpty) return null;

        return JsonConvert.DeserializeObject<T>(data!);
    }

    public async Task SaveAsync<T>(string key, T value) where T : class
    {
        await _database.StringSetAsync(key, JsonConvert.SerializeObject(value), TimeSpan.FromSeconds(_expire));
    }

    public async Task DeleteAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }
}