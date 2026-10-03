using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Friends;

public class RemoveFriendUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public RemoveFriendUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        if (currentUserId == userId)
        {
            throw new BadRequestException("Cannot remove friend to yourself.");
        }

        var friendship = await _context.Relationships
            .Friends()
            .Between(currentUserId, userId)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        _context.Relationships.Remove(friendship);
        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyAsync(userId, "friend.removed", new { UserId = currentUserId }, ct);
    }
}