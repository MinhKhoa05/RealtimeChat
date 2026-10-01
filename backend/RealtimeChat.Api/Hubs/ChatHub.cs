using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Application.Features.Presence;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IPresenceTracker _presenceTracker;
    private readonly UserOfflineUseCase _userOffline;
    private readonly UserOnlineUseCase _userOnline;

    public ChatHub(
        IPresenceTracker presenceTracker,
        UserOfflineUseCase userOffline,
        UserOnlineUseCase userOnline)
    {
        _presenceTracker = presenceTracker;
        _userOffline = userOffline;
        _userOnline = userOnline;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = long.Parse(Context.UserIdentifier!);

        var becameOnline = _presenceTracker.Connect(userId, Context.ConnectionId);

        if (becameOnline)
        {
            await _userOnline.ExecuteAsync(userId, Context.ConnectionAborted);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = long.Parse(Context.UserIdentifier!);
        
        var becameOffline = _presenceTracker.Disconnect(userId, Context.ConnectionId);

        if (becameOffline)
        {
            await _userOffline.ExecuteAsync(userId, Context.ConnectionAborted);
        }

        await base.OnDisconnectedAsync(exception);
    }
}