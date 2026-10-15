namespace RealtimeChat.Application.Features.Recommendations.CandidatePool;

public interface IPoolGenerator
{
    Task<List<Candidate>> GenerateAsync(long userId, int poolSize, HashSet<long> excludedUserIds, CancellationToken ct); 
}