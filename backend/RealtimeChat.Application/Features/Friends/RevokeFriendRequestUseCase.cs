using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Friends;

public class RevokeFriendRequestUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public RevokeFriendRequestUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long requestId, CancellationToken ct)
    {
        var friendRequest = await _context.Relationships
            .FriendRequest(requestId)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        if (_currentUser.UserId != friendRequest.UserId)
        {
            throw new ForbiddenException();
        }

        _context.Relationships.Remove(friendRequest);
        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyAsync(friendRequest.TargetUserId, "friend.request.revoked", new { FriendRequestId = friendRequest.Id }, ct);
    }
}