using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Servies;

public sealed class FriendRecommendationService : IFriendRecommendationService
{
    private readonly IAppDbContext _context;
    private readonly ICacheService _cache;

    private const int SampleSize = 10;
    private static readonly TimeSpan CooldownTtl = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan PoolTtl = TimeSpan.FromMinutes(30);
    private sealed record CandidateValue(long UserId, long Score);

    private static string PoolKey(long userId) => $"recommendations:{userId}";

    private static string CooldownKey(long userId) => $"{PoolKey(userId)}:cooldown";

    private static string SuppressedKey(long userId) => $"{PoolKey(userId)}:suppressed";

    public FriendRecommendationService(IAppDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IReadOnlyList<long>> GetAsync(long userId, CancellationToken ct)
    {
        var candidatePool = await GetCandidatePoolAsync(userId, ct);

        var candidates = Sample(candidatePool);

        // Kiểm tra
        var invalidCandidates = await FindInvalidCandidatesAsync(userId, candidates, ct);
        if (invalidCandidates.Count > 0)
        {
            await CleanCandidatePoolAsync(candidatePool, invalidCandidates);
            candidates.RemoveAll(x => invalidCandidates.Contains(x));
        }

        return candidates.Select(x => x.UserId).ToList();
    }

    private async Task<List<CandidateValue>> GetCandidatePoolAsync(long userId, CancellationToken ct)
    {
        if (await IsCooldownAsync(userId, ct)) return [];

        var candidatePool = await _cache.GetAsync<List<CandidateValue>>(PoolKey(userId), ct);

        if (candidatePool is null)
        {
            candidatePool = await RebuildPoolAsync(userId, ct);
        }

        return candidatePool;
    }

    private async Task<List<CandidateValue>> RebuildPoolAsync(long userId, CancellationToken ct)
    {
        return [];
    }

    private List<CandidateValue> Sample(List<CandidateValue> candidates)
    {
        return [];
    }

    private async Task CleanCandidatePoolAsync(List<CandidateValue> candidates, List<CandidateValue> invalidCandidates)
    {
        return;
    }

    private async Task<List<CandidateValue>> FindInvalidCandidatesAsync(long userId, List<CandidateValue> candidates, CancellationToken ct)
    {
        var candidateIds = candidates.Select(x => x.UserId).ToList();

        var relatedUserIds = await _context.Relationships
            .BetweenUsers(userId, candidateIds)
            .Select(x => x.UserId == userId ? x.TargetUserId : x.UserId)
            .ToListAsync(ct);

        var relatedUserIdSet = relatedUserIds.ToHashSet();

        return candidates.Where(x => relatedUserIdSet.Contains(x.UserId)).ToList();
    }

    private async Task<bool> IsCooldownAsync(long userId, CancellationToken ct)
        => await _cache.ExistsAsync(CooldownKey(userId), ct);
}