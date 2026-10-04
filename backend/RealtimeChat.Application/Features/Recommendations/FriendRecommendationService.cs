using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Recommendations;

public class FriendRecommendationService : IFriendRecommendationService
{
    private readonly IAppDbContext _context;
    private readonly ICandidatePoolManager _poolManager;

    private const int PoolMultiplier = 5;

    public FriendRecommendationService(IAppDbContext context, ICandidatePoolManager poolManager)
    {
        _context = context;
        _poolManager = poolManager;
    }

    public async Task<IReadOnlyList<long>> GetRecommendationsAsync(long userId, int limit, CancellationToken ct)
    {
        var pool = await GetOrCreateCandidatePoolAsync(userId, limit, ct);

        if (pool.Count == 0)
            return [];

        // Chọn ngẫu nhiên từ pool để tăng độ đa dạng cho Recommendation
        var candidates = Sample(pool, limit);

        // Lọc ra các candidate đã không còn hợp lệ.
        // Đồng thời loại chúng khỏi pool để những lần đề xuất sau không lấy lại.
        var invalidCandidateIds = await GetInvalidCandidateIdsAsync(userId, candidates, ct);
        if (invalidCandidateIds.Count > 0)
        {
            await _poolManager.PruneAsync(userId, invalidCandidateIds.ToHashSet(), ct);
            candidates.RemoveAll(x => invalidCandidateIds.Contains(x.UserId));
        }

        return candidates.Select(x => x.UserId).ToList();
    }

    #region Candidate Pool

    // Lấy hoặc tạo candidate pool và cache lại để tái sử dụng,
    // thay vì phải tạo mới mỗi lần cần đề xuất.
    private async Task<List<CandidateValue>> GetOrCreateCandidatePoolAsync(long userId, int limit, CancellationToken ct)
    {
        // Nếu đang trong cooldown, không tạo lại pool để tránh
        // phải tìm kiếm và query liên tục khi chưa có ứng viên phù hợp.
        if (await _poolManager.IsCooldownAsync(userId, ct))
            return [];

        var pool = await _poolManager.GetAsync(userId, ct);

        // Pool còn ứng viên thì dùng lại, không cần tìm lại từ đầu.
        if (pool is not null && pool.Count > 0)
            return pool;

        // Tạo pool lớn hơn số lượng cần trả về để có thêm nhiều sự lựa chọn khi Get
        var poolSize = limit * PoolMultiplier;

        var candidates = await TraverseCandidatesAsync(userId, poolSize, ct);

        // Nếu chưa tìm đủ ứng viên từ các mối quan hệ gián tiếp,
        // bố sung thêm user ngẫu nhiên từ hệ thống cho pool.
        if (candidates.Count < poolSize)
        {
            var need = poolSize - candidates.Count;

            var fallbackCandidates = await GetFallbackCandidatesAsync(userId, need, ct);

            candidates.AddRange(fallbackCandidates);
        }

        // Nếu vẫn không tìm được ứng viên nào thì đặt cooldown
        // để tránh phải tạo lại pool nhiều lần không cần thiết.
        if (candidates.Count == 0)
        {
            await _poolManager.SetCooldownAsync(userId, ct);
            return [];
        }

        await _poolManager.SetAsync(userId, candidates, ct);

        return candidates;
    }

    #endregion

    #region Candidate Generation

    // Sử dụng thuật toán BFS để tìm ứng viên phù hợp cho pool, mỗi tầng lấy những user đã tìm được
    // làm điểm trung gian để tìm thêm các user có liên hệ gián tiếp.
    // Chỉ mở rộng tối đa 2 tầng để tránh đi quá xa và tạo quá nhiều ứng viên.
    private async Task<List<CandidateValue>> TraverseCandidatesAsync(long userId, int limit, CancellationToken ct)
    {
        const int maxDepth = 2;
        const int maxExpansionSources = 10;

        var scores = new Dictionary<long, int>();
        var expanded = new HashSet<long> { userId };
        var frontier = new List<long> { userId };

        for (var depth = 0; depth < maxDepth; depth++)
        {
            if (frontier.Count == 0 || scores.Count >= limit)
                break;

            // Chỉ mở rộng từ một số source có score cao nhất.
            var sourceUserIds = frontier
                .Take(maxExpansionSources)
                .ToList();

            var candidates = await FindCandidatesAsync(userId, sourceUserIds, limit, ct);

            // Cộng dồn score qua các tầng, tầng càng cao thì score càng giảm do quan hệ gián tiếp.
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
            .Take(limit)
            .Select(x => new CandidateValue(x.Key, x.Value))
            .ToList();
    }

    // Tìm ứng viên dựa trên các conversation mà các user cùng tham gia.
    // Dựa vào số conversation chung để ước lượng khả năng quen biết:
    // càng có nhiều conversation chung thì khả năng quen biết càng cao.
    //
    // userId là người cần tìm đề xuất.
    // Source users là những user đã tìm được ở các bước trước, được lưu tạm thời
    // để tiếp tục tìm thêm những user có liên hệ gián tiếp.
    // Ví dụ: userId -> userA -> userB. UserB có thể không có conversation chung với userId,
    // nhưng vẫn có thể được tìm thấy thông qua userA. Nhờ đó candidate pool được mở rộng hơn,
    // thay vì chỉ lấy những user có conversation chung trực tiếp với userId.
    //
    // Những user đã có Relationship với userId sẽ được bỏ qua để tránh đề xuất trùng.
    private async Task<List<CandidateValue>> FindCandidatesAsync(long userId, List<long> sourceUserIds, int limit, CancellationToken ct)
    {
        // Lấy các conversation mà source users tham gia.
        var conversationIds = _context.ConversationMembers
            .AsNoTracking()
            .Where(x => sourceUserIds.Contains(x.MemberId))
            .Select(x => x.ConversationId);

        // Lấy các thành viên khác trong những conversation đó.
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

        // Loại bỏ các thành viên đã có Relationship với user.
        var candidateIds = candidateConnections
            .Where(x => !_context.Relationships.Between(userId, x.MemberId).Any());

        // Tính Weight dựa trên số conversation chung và lấy các ứng viên có Weight cao nhất.
        return await candidateIds
            .GroupBy(x => x.MemberId)
            .Select(g => new CandidateValue(g.Key, g.Count()))
            .OrderByDescending(x => x.Weight)
            .Take(limit)
            .ToListAsync(ct);
    }

    // Khi số lượng candidate có conversation không nhiều, thực hiện lấy ngẫu nhiên các user có trong hệ thống để đề xuất
    private async Task<List<CandidateValue>> GetFallbackCandidatesAsync(long userId, int limit, CancellationToken ct)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x =>
                x.Id != userId &&
                !_context.Relationships.Between(userId, x.Id).Any())
            .OrderBy(x => EF.Functions.Random()) // Có thể gây chậm khi hệ thống lớn.
            .Select(x => new CandidateValue(x.Id, 1))
            .Take(limit)
            .ToListAsync(ct);
    }

    #endregion

    #region Candidate Validation

    private async Task<List<long>> GetInvalidCandidateIdsAsync(
        long userId,
        List<CandidateValue> candidates,
        CancellationToken ct)
    {
        var candidateIds = candidates
            .Select(x => x.UserId)
            .ToHashSet();

        var invalidCandidateIds = await _context.Relationships
            .BetweenUsers(userId, candidateIds)
            .Select(x =>
                x.UserId == userId
                    ? x.TargetUserId
                    : x.UserId)
            .ToListAsync(ct);

        // TODO: Thêm check Suppressed

        return invalidCandidateIds;
    }

    #endregion

    #region Sampling

    private static List<CandidateValue> Sample(List<CandidateValue> candidatePool, int limit)
    {
        return candidatePool.OrderBy(_ => Random.Shared.Next()).Take(limit).ToList();
    }

    #endregion
}