using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Api.Hubs;

public class SignalRNotifier : IClientNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyAsync<T>(long userId, string eventName, T data, CancellationToken ct)
    {
        return _hubContext.Clients.Users(userId.ToString()).SendAsync(eventName, data, ct);
    }

    public Task NotifyAsync<T>(IReadOnlyCollection<long> userIds, string eventName, T data, CancellationToken ct)
    {
        return _hubContext.Clients.Users(userIds.Select(x => x.ToString())).SendAsync(eventName, data, ct);
    }

    public Task NotifyToConversationAsync<T>(long conversationId, string eventName, T data, CancellationToken ct)
    {
        return _hubContext.Clients.Group($"conversation:{conversationId}").SendAsync(eventName, data, ct);
    }
}