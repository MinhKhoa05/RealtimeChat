namespace RealtimeChat.Application.Interfaces;

public interface IFriendRecommendationService
{
    Task<IReadOnlyList<long>> GetAsync(long userId, CancellationToken ct);
}