using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Features.Recommendations.CandidatePool;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Recommendations.Services;

public class FriendRecommendationService : IFriendRecommendationService
{
    private readonly IAppDbContext _context;
    private readonly IPoolCache _poolCache;
    private readonly IPoolGenerator _poolGenerator;

    private const int PoolMultiplier = 5;

    public FriendRecommendationService(IAppDbContext context, IPoolCache poolCache, IPoolGenerator poolGenerator)
    {
        _context = context;
        _poolCache = poolCache;
        _poolGenerator = poolGenerator;
    }

    public async Task<IReadOnlyList<long>> GetRecommendationsAsync(long userId, int limit, CancellationToken ct)
    {
        var pool = await _poolCache.GetPoolAsync(userId, ct);

        if (pool.Count == 0)
        {
            pool = await RebuildPoolAsync(userId, limit, ct);
        }

        // Rebuild xong mà vẫn không có, dừng lun
        if (pool.Count == 0)
            return [];

        // Chọn ngẫu nhiên để tăng độ đa dạng.
        var candidateIds = Sample(pool, limit);

        // Loại các candidate không còn hợp lệ khỏi kết quả và pool.
        var invalidIds = await FilterInvalidCandidateIdsAsync(userId, candidateIds, ct);

        if (invalidIds.Count > 0)
        {
            candidateIds.RemoveWhere(invalidIds.Contains);
            pool.RemoveAll(x => invalidIds.Contains(x.UserId));

            await _poolCache.SetPoolAsync(userId, pool, ct);
        }

        return candidateIds.ToList();
    }

    private static HashSet<long> Sample(List<Candidate> pool, int limit)
    {
        return pool.OrderBy(_ => Random.Shared.Next()).Take(limit).Select(x => x.UserId).ToHashSet();
    }

    private async Task<List<Candidate>> RebuildPoolAsync(long userId, int limit, CancellationToken ct)
    {
        if (await _poolCache.IsCooldownAsync(userId, ct))
            return [];

        // Tạo pool lớn hơn số lượng cần trả về để có thêm lựa chọn khi Get.
        var poolSize = limit * PoolMultiplier;

        var suppressedUserIds = await _poolCache.GetSuppressedAsync(userId, ct) ?? [];
        var candidates = await _poolGenerator.GenerateAsync(userId, poolSize, suppressedUserIds.ToHashSet(), ct);

        // Không tìm được ứng viên -> đặt cooldown để tạm thời không rebuild lại pool.
        if (candidates.Count == 0)
        {
            await _poolCache.SetCooldownAsync(userId, ct);
            return [];
        }

        // Cache lại để tái sử dụng pool
        await _poolCache.SetPoolAsync(userId, candidates, ct);

        return candidates;
    }

    private async Task<HashSet<long>> FilterInvalidCandidateIdsAsync(long userId, HashSet<long> candidateIds, CancellationToken ct)
    {
        var invalidCandidateIds = await _context.Relationships
            .BetweenUsers(userId, candidateIds)
            .Select(x =>
                x.UserId == userId
                    ? x.TargetUserId
                    : x.UserId)
            .ToListAsync(ct);

        var invalidCandidateIdsSet = invalidCandidateIds.ToHashSet();

        var suppressedUserIds = await _poolCache.GetSuppressedAsync(userId, ct);
        if (suppressedUserIds.Count != 0)
        {
            invalidCandidateIdsSet.UnionWith(suppressedUserIds.Where(candidateIds.Contains));
        }

        return invalidCandidateIdsSet;
    }
}