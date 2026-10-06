namespace RealtimeChat.Application.Features.Recommendations.Services;

public interface IFriendRecommendationService
{
    Task<IReadOnlyList<long>> GetRecommendationsAsync(long userId, int limit, CancellationToken ct);
}