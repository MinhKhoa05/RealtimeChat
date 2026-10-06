namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public interface IPoolCache
{
    Task<List<Candidate>> GetPoolAsync(long userId, CancellationToken ct);

    Task SetPoolAsync(long userId, List<Candidate> pool, CancellationToken ct);

    Task<bool> IsCooldownAsync(long userId, CancellationToken ct);

    Task SetCooldownAsync(long userId, CancellationToken ct);

    Task<List<long>> GetSuppressedAsync(long userId, CancellationToken ct);
    Task AddSuppressedAsync(long userId, long suppressedUserId, CancellationToken ct);
}