namespace RealtimeChat.Application.Interfaces;

public interface IClientNotifier
{
    Task NotifyAsync<T>(long userId, string eventName, T data, CancellationToken ct = default);
    Task NotifyAsync<T>(IReadOnlyCollection<long> userIds, string eventName, T data, CancellationToken ct = default);
    Task NotifyToConversationAsync<T>(long conversationId, string eventName, T data, CancellationToken ct = default);
}