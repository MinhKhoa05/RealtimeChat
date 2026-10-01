namespace RealtimeChat.Application.Interfaces;

public interface IPresenceTracker
{
    bool Connect(long userId, string connectionId);
    bool Disconnect(long userId, string connectionId);
    bool IsOnline(long userId);

    IReadOnlySet<long> GetOnlineUserIds(IEnumerable<long> userIds);
}