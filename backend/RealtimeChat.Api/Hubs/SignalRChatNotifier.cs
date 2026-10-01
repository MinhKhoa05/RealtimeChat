using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Api.Hubs;

public class SignalRChatNotifier : IChatNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRChatNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task Notify<T>(long userId, string eventName, T data, CancellationToken ct)
    {
        return _hubContext.Clients.Users(userId.ToString()).SendAsync(eventName, data, ct);
    }

    public Task Notify<T>(IReadOnlyCollection<long> userIds, string eventName, T data, CancellationToken ct)
    {
        return _hubContext.Clients.Users(userIds.Select(x => x.ToString())).SendAsync(eventName, data, ct);
    }

    public Task NotifyToConversation<T>(long conversationId, string eventName, T data, CancellationToken ct)
    {
        return _hubContext.Clients.Group($"conversation:{conversationId}").SendAsync(eventName, data, ct);
    }
}