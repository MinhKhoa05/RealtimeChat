using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public class PoolGenerator : IPoolGenerator
{
    private readonly IAppDbContext _context;

    public PoolGenerator(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Candidate>> GenerateAsync(long userId, int poolSize, IReadOnlySet<long> excludedUserIds, CancellationToken ct)
    {
        var candidates = await TraverseCandidatesAsync(userId, poolSize, excludedUserIds, ct);
        if (candidates.Count < poolSize)
        {
            var fallbackExcludedUserIds = new HashSet<long>(excludedUserIds);

            foreach (var candidate in candidates)
                fallbackExcludedUserIds.Add(candidate.UserId);

            var fallbackCandidates = await GetFallbackCandidatesAsync(userId, poolSize - candidates.Count, fallbackExcludedUserIds, ct);

            candidates.AddRange(fallbackCandidates);
        }

        return candidates;
    }

    // Duyệt ứng viên theo BFS qua các conversation chung.
    // Mỗi tầng sử dụng các user tìm được làm cầu nối cho tầng tiếp theo.
    // Giới hạn tối đa 2 tầng để kiểm soát phạm vi tìm kiếm.
    private async Task<List<Candidate>> TraverseCandidatesAsync(long userId, int poolSize, IReadOnlySet<long> excludedUserIds, CancellationToken ct)
    {
        const int maxDepth = 2;
        const int maxExpansionSources = 10;

        var scores = new Dictionary<long, int>();
        var expanded = new HashSet<long> { userId };
        var frontier = new List<long> { userId };

        for (var depth = 0; depth < maxDepth; depth++)
        {
            if (frontier.Count == 0 || scores.Count >= poolSize)
                break;

            // Chỉ mở rộng từ một số source có score cao nhất.
            var sourceUserIds = frontier
                .Take(maxExpansionSources)
                .ToList();

            var candidates = await FindCandidatesAsync(userId, sourceUserIds, poolSize, excludedUserIds, ct);

            // Giảm trọng số theo độ sâu để ưu tiên các mối liên hệ gần hơn.
            var depthWeight = 1 << (maxDepth - depth);

            foreach (var candidate in candidates)
            {
                scores[candidate.UserId] = scores.GetValueOrDefault(candidate.UserId) + candidate.Weight * depthWeight;
            }

            // Chọn candidate có score cao làm source cho tầng tiếp theo.
            frontier = candidates
                .OrderByDescending(x => x.Weight)
                .Select(x => x.UserId)
                .Where(expanded.Add)
                .Take(maxExpansionSources)
                .ToList();
        }

        return scores
            .OrderByDescending(x => x.Value)
            .Take(poolSize)
            .Select(x => new Candidate(x.Key, x.Value))
            .ToList();
    }

    // Tìm ứng viên dựa trên các conversation mà source users cùng tham gia.
    // User có nhiều conversation chung hơn sẽ có Weight cao hơn.
    private async Task<List<Candidate>> FindCandidatesAsync(long userId, List<long> sourceUserIds, int limit, IReadOnlySet<long> excludedUserIds, CancellationToken ct)
    {
        var conversationIds = _context.ConversationMembers
            .AsNoTracking()
            .Where(x => sourceUserIds.Contains(x.MemberId))
            .Select(x => x.ConversationId);

        var candidateConnections = _context.ConversationMembers
            .AsNoTracking()
            .Where(x =>
                conversationIds.Contains(x.ConversationId) &&
                !sourceUserIds.Contains(x.MemberId))
            .Select(x => new
            {
                x.MemberId,
                x.ConversationId
            })
            .Distinct();

        var candidateIds = candidateConnections.Where(x => !excludedUserIds.Contains(x.MemberId));

        candidateIds = candidateIds.Where(x => !_context.Relationships.Between(userId, x.MemberId).Any());

        return await candidateIds
            .GroupBy(x => x.MemberId)
            .Select(g => new Candidate(g.Key, g.Count()))
            .OrderByDescending(x => x.Weight)
            .Take(limit)
            .ToListAsync(ct);
    }

    // Bổ sung user ngẫu nhiên nếu Traversal chưa đủ pool.
    private async Task<List<Candidate>> GetFallbackCandidatesAsync(long userId, int limit, IReadOnlySet<long> excludedUserIds, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x =>
                x.Id != userId &&
                !_context.Relationships.Between(userId, x.Id).Any() &&
                !excludedUserIds.Contains(x.Id))
            .OrderBy(x => EF.Functions.Random()) // Có thể gây chậm khi hệ thống lớn.
            .Select(x => new Candidate(x.Id, 1))
            .Take(limit)
            .ToListAsync(ct);
    }
}