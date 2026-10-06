using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public class PoolCache : IPoolCache
{
    private readonly ICacheService _cache;

    private static readonly TimeSpan CooldownTtl = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan PoolTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan SuppressedTtl = TimeSpan.FromDays(1);

    private static string PoolKey(long userId) => $"recommendations:{userId}";
    private static string CooldownKey(long userId) => $"{PoolKey(userId)}:cooldown";
    private static string SuppressedKey(long userId) => $"{PoolKey(userId)}:suppressed";

    public PoolCache(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<List<Candidate>> GetPoolAsync(long userId, CancellationToken ct)
    {
        return await _cache.GetAsync<List<Candidate>>(PoolKey(userId), ct) ?? [];
    }

    public async Task SetPoolAsync(long userId, List<Candidate> pool, CancellationToken ct)
    {
        await _cache.SetAsync(PoolKey(userId), pool, PoolTtl, ct);
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

    public async Task<List<long>> GetSuppressedAsync(long userId, CancellationToken ct)
    {
        return await _cache.GetAsync<List<long>>(SuppressedKey(userId), ct) ?? [];
    }

    public async Task AddSuppressedAsync(long userId, long suppressedUserId, CancellationToken ct)
    {
        var suppressedUserIds = await GetSuppressedAsync(userId, ct);

        if (suppressedUserIds.Contains(suppressedUserId))
            return;

        suppressedUserIds.Add(suppressedUserId);

        // Mỗi lần thêm user mới sẽ reset lại thời gian hết hạn
        await _cache.SetAsync(SuppressedKey(userId), suppressedUserIds, SuppressedTtl, ct);
    }
}