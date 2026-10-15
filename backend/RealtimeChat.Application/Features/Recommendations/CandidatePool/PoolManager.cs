using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public class PoolManager : IPoolManager
{
    private readonly ICacheService _cache;
    private readonly IPoolGenerator _generator;

    private const int MaxPoolSize = 50;

    private static readonly TimeSpan CooldownTtl = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan PoolTtl = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan SuppressedTtl = TimeSpan.FromDays(1);

    private static string PoolKey(long userId) => $"recommendations:{userId}";
    private static string CooldownKey(long userId) => $"{PoolKey(userId)}:cooldown";
    private static string SuppressedKey(long userId) => $"{PoolKey(userId)}:suppressed";

    public PoolManager(ICacheService cache, IPoolGenerator generator)
    {
        _cache = cache;
        _generator = generator;
    }

    public async Task<List<Candidate>> GetOrCreateAsync(long userId, CancellationToken ct)
    {
        var pool = await _cache.GetAsync<List<Candidate>>(PoolKey(userId), ct) ?? [];

        if (pool.Count == 0)
        {
            pool = await RebuildAsync(userId, ct);
        }

        return pool;
    }

    public async Task PruneAsync(long userId, IReadOnlySet<long> invalidUserIds, CancellationToken ct)
    {
        if (invalidUserIds.Count == 0)
            return;

        var pool = await _cache.GetAsync<List<Candidate>>(PoolKey(userId), ct) ?? [];

        if (pool.Count == 0)
            return;

        var removedCount = pool.RemoveAll(x => invalidUserIds.Contains(x.UserId));

        if (removedCount == 0)
            return;

        // Prune sẽ cập nhật lại cache và thời gian TTL
        await _cache.SetAsync(PoolKey(userId), pool, PoolTtl, ct);
    }

    private async Task<List<Candidate>> RebuildAsync(long userId, CancellationToken ct)
    {
        // Đang cooldown thì không tạo nữa
        var cooldown = await _cache.ExistsAsync(CooldownKey(userId), ct);
        if (cooldown) return [];

        var suppressedUserIds = await GetSuppressedAsync(userId, ct);
        var candidates = await _generator.GenerateAsync(userId, MaxPoolSize, suppressedUserIds.ToHashSet(), ct);

        // Không tìm được ứng viên -> đặt cooldown để lần sau không rebuild lại pool.
        if (candidates.Count == 0)
        {
            // Set cooldown
            await _cache.SetAsync(CooldownKey(userId), true, CooldownTtl, ct);
            await _cache.RemoveAsync(PoolKey(userId));
            return [];
        }

        // Cache lại để tái sử dụng pool
        await _cache.SetAsync(PoolKey(userId), candidates, PoolTtl, ct);

        return candidates;
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