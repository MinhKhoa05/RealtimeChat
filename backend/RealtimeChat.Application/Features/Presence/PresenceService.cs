using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Presence;

public class PresenceService : IPresenceService
{
    private readonly IAppDbContext _context;
    private readonly IPresenceTracker _tracker;
    private readonly IClientNotifier _notifier;

    private readonly TimeProvider _timeProvider;

    public PresenceService(IAppDbContext context, IPresenceTracker tracker, IClientNotifier notifier, TimeProvider timeProvider)
    {
        _context = context;
        _tracker = tracker;
        _notifier = notifier;
        _timeProvider = timeProvider;
    }

    public async Task ConnectAsync(long userId, string connectionId, CancellationToken ct = default)
    {
        var becameOnline = _tracker.Connect(userId, connectionId);

        if (becameOnline)
        {
            await NotifyToFriendsAsync(userId, "user.online", ct);
        }
    }

    public async Task DisconnectAsync(long userId, string connectionId, CancellationToken ct = default)
    {
        var becameOffline = _tracker.Disconnect(userId, connectionId);

        if (becameOffline)
        {
            var user = await _context.Users.FindAsync(userId, ct);
            if (user == null)
            {
                return;
            }

            user.MaskLastSeen(_timeProvider.GetUtcNow().UtcDateTime);
            await _context.SaveChangesAsync(ct);

            await NotifyToFriendsAsync(userId, "user.offline", ct);
        }
    }

    private async Task NotifyToFriendsAsync(long userId, string eventName, CancellationToken ct)
    {
        var friendIds = await _context.Relationships
            .AsNoTracking()
            .Friends()
            .OfUsers(userId)
            .Select(x => x.UserId == userId ? x.TargetUserId : x.UserId)
            .ToListAsync(ct);

        await _notifier.NotifyAsync(friendIds, eventName, new { UserId = userId }, ct);
    }
}