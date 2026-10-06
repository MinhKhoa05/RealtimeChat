namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public interface IPoolGenerator
{
    Task<List<Candidate>> GenerateAsync(long userId, int poolSize, IReadOnlySet<long> excludedUserIds, CancellationToken ct); 
}