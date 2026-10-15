using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public class PoolGenerator : IPoolGenerator
{
    private readonly IAppDbContext _context;

    private const int SharedConversationWeight = 3;
    private const int MutualFriendWeight = 5; // Ưu tiên quan hệ bạn chung hơn
    private const int RandomWeight = 0;

    public PoolGenerator(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Candidate>> GenerateAsync(long userId, int poolSize, HashSet<long> excludedUserIds, CancellationToken ct)
    {
        if (poolSize <= 0)
            return [];

        var candidates = new Dictionary<long, Candidate>();

        // Lấy từ nhóm chung
        var sharedCandidates = await GetSharedConversationCandidatesAsync(userId, poolSize, excludedUserIds, ct);
        MergeCandidates(candidates, sharedCandidates);

        // Lấy từ bạn của bạn bè
        var friendsOfFriendsCandidates = await GetMutualFriendCandidatesAsync(userId, poolSize, excludedUserIds, ct);
        MergeCandidates(candidates, friendsOfFriendsCandidates);

        // Nếu chưa đủ thì lấy random
        if (candidates.Count < poolSize)
        {
            foreach (var candidate in candidates)
            {
                excludedUserIds.Add(candidate.Key);
            }

            var randomCandidates = await GetRandomCandidatesAsync(userId, poolSize - candidates.Count, excludedUserIds, ct);
            MergeCandidates(candidates, randomCandidates);
        }

        // Xếp hạng và lấy top N ứng viên.
        return candidates.Values
            .OrderByDescending(x => x.Weight)
            .Take(poolSize)
            .ToList();
    }

    private async Task<List<Candidate>> GetSharedConversationCandidatesAsync(long userId, int limit, HashSet<long> excludedUserIds, CancellationToken ct)
    {
        // Lấy các conversation đang hoạt động mà user tham gia.
        var conversationIds = _context.Conversations
            .AsNoTracking()
            .Active()
            .AccessibleBy(userId)
            .Select(x => x.Id);

        // Lấy danh sách user đủ điều kiện để gợi ý.
        var eligibleUserIds = GetEligibleUserIds(userId, excludedUserIds);

        // Lấy các thành viên đủ điều kiện trong conversation chung.
        var sharedMembers = _context.ConversationMembers
            .AsNoTracking()
            .Where(x => conversationIds.Contains(x.ConversationId))
            .Where(x => eligibleUserIds.Contains(x.MemberId));

        // Xếp hạng theo số conversation chung.
        return await sharedMembers
            .GroupBy(x => x.MemberId)
            .Select(g => new Candidate(g.Key, g.Count() * SharedConversationWeight))
            .OrderByDescending(x => x.Weight)
            .Take(limit)
            .ToListAsync(ct);
    }

    private async Task<List<Candidate>> GetMutualFriendCandidatesAsync(long userId, int limit, HashSet<long> excludedUserIds, CancellationToken ct)
    {
        // Lấy danh sách bạn bè trực tiếp của user.
        var friendIds = _context.Relationships
            .Friends()
            .OfUsers(userId)
            .Select(x => x.UserId == userId ? x.TargetUserId : x.UserId);

        // Lấy danh sách user đủ điều kiện để gợi ý.
        var eligibleUserIds = GetEligibleUserIds(userId, excludedUserIds);

        // Lấy ứng viên từ bạn bè của bạn bè trực tiếp.
        var friendOfFriendIds = _context.Relationships
            .Friends()
            .Where(x =>
                friendIds.Contains(x.UserId) ||
                friendIds.Contains(x.TargetUserId))
            .Select(x =>
                friendIds.Contains(x.UserId) ? x.TargetUserId : x.UserId)
            .Where(x => eligibleUserIds.Contains(x));

        // Đếm số bạn chung của mỗi ứng viên.
        return await friendOfFriendIds
            .GroupBy(x => x)
            .Select(g => new Candidate(g.Key, g.Count() * MutualFriendWeight))
            .OrderByDescending(x => x.Weight)
            .Take(limit)
            .ToListAsync(ct);
    }

    private async Task<List<Candidate>> GetRandomCandidatesAsync(long userId, int limit, HashSet<long> excludedUserIds, CancellationToken ct)
    {
        // Lấy danh sách user đủ điều kiện để gợi ý.
        var eligibleUserIds = GetEligibleUserIds(userId, excludedUserIds);

        // Lấy ngẫu nhiên các ứng viên.
        return await eligibleUserIds
            .OrderBy(_ => EF.Functions.Random()) // Có thể gây chậm khi có nhiều users.
            .Take(limit)
            .Select(x => new Candidate(x, RandomWeight))
            .ToListAsync(ct);
    }

    private static void MergeCandidates(Dictionary<long, Candidate> candidates, IEnumerable<Candidate> incoming)
    {
        foreach (var candidate in incoming)
        {
            if (candidates.TryGetValue(candidate.UserId, out var existing))
            {
                // Nếu bị trùng thì cộng điểm thêm weight
                candidates[candidate.UserId] = existing with
                {
                    Weight = existing.Weight + candidate.Weight
                };
            }
            else
            {
                candidates[candidate.UserId] = candidate;
            }
        }
    }

    // Subquery chứa các UserId đủ điều kiện để gợi ý kết bạn.
    private IQueryable<long> GetEligibleUserIds(long userId, HashSet<long> excludedUserIds)
    {
        return _context.Users
            .AsNoTracking()
            .Where(x => x.Id != userId)
            .Where(x => !excludedUserIds.Contains(x.Id))
            .Where(x => !_context.Relationships.Between(userId, x.Id).Any())
            .Select(x => x.Id);
    }
}