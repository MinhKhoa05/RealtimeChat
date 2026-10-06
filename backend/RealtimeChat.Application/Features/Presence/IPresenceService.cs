namespace RealtimeChat.Application.Features.Presence;

public interface IPresenceService
{
    Task ConnectAsync(long userId, string connectionId, CancellationToken ct = default);
    Task DisconnectAsync(long userId, string connectionId, CancellationToken ct = default);
}