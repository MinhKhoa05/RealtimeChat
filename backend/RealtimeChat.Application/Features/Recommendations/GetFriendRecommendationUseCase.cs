using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Application.Features.Recommendations.CandidatePool;

namespace RealtimeChat.Application.Features.Recommendations;

public class GetFriendRecommendationUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IPoolManager _poolManager;

    public GetFriendRecommendationUseCase(IAppDbContext context, ICurrentUser currentUser, IPoolManager poolManager)
    {
        _context = context;
        _currentUser = currentUser;
        _poolManager = poolManager;
    }

    public async Task<IReadOnlyList<long>> ExecuteAsync(int limit, CancellationToken ct)
    {
        if (limit < 1 || limit > 7)
            throw new BadRequestException("Limit must be from 1 - 15");

        var userId = _currentUser.UserId;

        var pool = await _poolManager.GetOrCreateAsync(userId, ct);

        if (pool.Count == 0)
            return [];

        // Chọn ngẫu nhiên để tăng độ đa dạng.
        var candidateIds = pool
            .OrderBy(_ => Random.Shared.Next())
            .Take(limit)
            .Select(x => x.UserId)
            .ToHashSet();

        // Loại các candidate không còn hợp lệ khỏi kết quả và pool.
        var invalidIds = await GetInvalidCandidateIdsAsync(userId, candidateIds, ct);
        if (invalidIds.Count > 0)
        {
            candidateIds.RemoveWhere(invalidIds.Contains);
            await _poolManager.PruneAsync(userId, invalidIds, ct);
        }

        return candidateIds.ToList();
    }

    private async Task<HashSet<long>> GetInvalidCandidateIdsAsync(long userId, HashSet<long> candidateIds, CancellationToken ct)
    {
        var invalidIds = (await _context.Relationships
            .BetweenUsers(userId, candidateIds)
            .Select(x => x.UserId == userId ? x.TargetUserId : x.UserId)
            .ToListAsync(ct))
            .ToHashSet();

        var suppressedIds = await _poolManager.GetSuppressedAsync(userId, ct);

        invalidIds.UnionWith(suppressedIds.Where(candidateIds.Contains));

        return invalidIds;
    }
}