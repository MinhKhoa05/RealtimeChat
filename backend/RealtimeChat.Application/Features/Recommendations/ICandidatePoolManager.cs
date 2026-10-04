namespace RealtimeChat.Application.Features.Recommendations;

public interface ICandidatePoolManager
{
    Task<List<CandidateValue>?> GetAsync(long userId, CancellationToken ct);

    Task SetAsync(long userId, List<CandidateValue> pool, CancellationToken ct);

    Task PruneAsync(long userId, HashSet<long> invalidCandidateIds, CancellationToken ct);

    Task<bool> IsCooldownAsync(long userId, CancellationToken ct);

    Task SetCooldownAsync(long userId, CancellationToken ct);
}