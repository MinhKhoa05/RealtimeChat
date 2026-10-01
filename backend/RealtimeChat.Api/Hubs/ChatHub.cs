using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RealtimeChat.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
}