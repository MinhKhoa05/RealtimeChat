using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Application.Features.Presence;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IPresenceService _presenceService;

    public ChatHub(IPresenceService presenceService)
    {
        _presenceService = presenceService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = long.Parse(Context.UserIdentifier!);

        await _presenceService.ConnectAsync(userId, Context.ConnectionId, Context.ConnectionAborted);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = long.Parse(Context.UserIdentifier!);

        await _presenceService.DisconnectAsync(userId, Context.ConnectionId, Context.ConnectionAborted);

        await base.OnDisconnectedAsync(exception);
    }
}