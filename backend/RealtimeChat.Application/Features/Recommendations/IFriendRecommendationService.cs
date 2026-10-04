namespace RealtimeChat.Application.Features.Recommendations;

public interface IFriendRecommendationService
{
    Task<IReadOnlyList<long>> GetRecommendationsAsync(long userId, int limit, CancellationToken ct);
}