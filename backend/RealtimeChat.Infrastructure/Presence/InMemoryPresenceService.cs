using System.Collections.Concurrent;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Infrastructure.Presence;

public class InMemoryPresenceTracker : IPresenceTracker
{
    private readonly ConcurrentDictionary<long, ConcurrentDictionary<string, byte>> _connections = new();

    public bool Connect(long userId, string connectionId)
    {
        var connections = _connections.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());

        var becameOnline = connections.IsEmpty;

        connections.TryAdd(connectionId, 0);

        return becameOnline;
    }

    public bool Disconnect(long userId, string connectionId)
    {
        if (!_connections.TryGetValue(userId, out var connections)) return false;

        connections.TryRemove(connectionId, out _);

        var becameOffline = connections.IsEmpty;
        
        if (becameOffline)
        {
            _connections.TryRemove(userId, out _);
        }

        return becameOffline;
    }

    public bool IsOnline(long userId)
    {
        return _connections.ContainsKey(userId);
    }

    public IReadOnlySet<long> GetOnlineUserIds(IEnumerable<long> userIds)
    {
        return userIds.Where(IsOnline).ToHashSet();
    }
}