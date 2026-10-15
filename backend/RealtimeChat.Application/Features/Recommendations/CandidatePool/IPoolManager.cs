namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public interface IPoolManager
{
    Task<List<Candidate>> GetOrCreateAsync(long userId, CancellationToken ct);
    Task PruneAsync(long userId, IReadOnlySet<long> invalidUserIds, CancellationToken ct);

    Task<List<long>> GetSuppressedAsync(long userId, CancellationToken ct);
    Task AddSuppressedAsync(long userId, long suppressedUserId, CancellationToken ct);
}