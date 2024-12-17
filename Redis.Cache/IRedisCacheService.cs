namespace Redis.Cache;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SaveAsync<T>(string key, T value) where T : class;
    Task DeleteAsync(string key);
}