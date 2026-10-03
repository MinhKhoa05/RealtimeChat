using Microsoft.Extensions.Caching.Memory;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Infrastructure.Caching;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public InMemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        return Task.FromResult(_cache.Get<T>(key));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }

        _cache.Set(key, value, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _cache.Remove(key);

        return Task.CompletedTask;
    }
}