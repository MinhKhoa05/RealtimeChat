using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Presence;

public class UserOnlineUseCase
{
    private readonly IAppDbContext _context;
    private readonly IClientNotifier _notifier;

    public UserOnlineUseCase(IAppDbContext context, IClientNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long userId, CancellationToken ct)
    {
        var friendIds = await _context.Relationships
            .Friends()
            .OfUsers(userId)
            .Select(x => x.UserId == userId ? x.TargetUserId : x.UserId)
            .ToListAsync(ct);

        await _notifier.NotifyAsync(friendIds, "user.online", new UserPresenceNotification { UserId = userId }, ct);
    }
}