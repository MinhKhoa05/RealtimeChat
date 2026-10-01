namespace RealtimeChat.Application.Interfaces;

public interface IChatNotifier
{
    Task Notify<T>(long userId, string eventName, T data, CancellationToken ct = default);
    Task Notify<T>(IReadOnlyCollection<long> userIds, string eventName, T data, CancellationToken ct = default);
    Task NotifyToConversation<T>(long conversationId, string eventName, T data, CancellationToken ct = default);
}