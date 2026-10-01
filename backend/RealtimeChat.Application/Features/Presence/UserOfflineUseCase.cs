using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Presence;

public class UserOfflineUseCase
{
    private readonly IAppDbContext _context;
    private readonly IClientNotifier _notifier;

    public UserOfflineUseCase(IAppDbContext context, IClientNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long userId, CancellationToken ct)
    {
        var user = await _context.Users.FindAsync(userId, ct)
            ?? throw new Exception("User not found");

        user.LastSeenAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        var friendIds = await _context.Relationships
            .Friends()
            .Where(x => x.UserId == userId || x.TargetUserId == userId)
            .Select(x => x.UserId == userId ? x.TargetUserId : x.UserId)
            .ToListAsync(ct);

        await _notifier.NotifyAsync(friendIds, "user.offline", new UserPresenceNotification { UserId = userId }, ct);
    }
}

public class UserPresenceNotification
{
    public long UserId { get; set; }
}