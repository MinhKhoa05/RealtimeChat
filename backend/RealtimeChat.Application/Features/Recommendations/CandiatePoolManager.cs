using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Recommendations;

public class CandidatePoolManager : ICandidatePoolManager
{
    private readonly ICacheService _cache;

    private static readonly TimeSpan CooldownTtl = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan PoolTtl = TimeSpan.FromMinutes(30);

    private static string PoolKey(long userId) => $"recommendations:{userId}";
    private static string CooldownKey(long userId) => $"{PoolKey(userId)}:cooldown";

    public CandidatePoolManager(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<List<CandidateValue>?> GetAsync(long userId, CancellationToken ct)
    {
        return await _cache.GetAsync<List<CandidateValue>>(PoolKey(userId), ct);
    }

    public async Task SetAsync(long userId, List<CandidateValue> pool, CancellationToken ct)
    {
        await _cache.SetAsync(PoolKey(userId), pool, PoolTtl, ct);
    }

    public async Task PruneAsync(long userId, HashSet<long> invalidCandidateIds, CancellationToken ct)
    {
        var pool = await GetAsync(userId, ct);

        if (pool is null) return;

        pool.RemoveAll(x => invalidCandidateIds.Contains(x.UserId));

        if (pool.Count == 0)
        {
            await _cache.RemoveAsync(PoolKey(userId));
            return;
        }

        // Reset TTL vì pool vừa được sử dụng.
        // TTL chỉ dùng để kiểm soát vòng đời của pool trong cache.
        await SetAsync(userId, pool, ct);
    }

    public async Task<bool> IsCooldownAsync(long userId, CancellationToken ct)
    {
        return await _cache.ExistsAsync(CooldownKey(userId), ct);
    }

    public async Task SetCooldownAsync(long userId, CancellationToken ct)
    {
        await _cache.SetAsync(CooldownKey(userId), true, CooldownTtl, ct);

        await _cache.RemoveAsync(PoolKey(userId));
    }
}