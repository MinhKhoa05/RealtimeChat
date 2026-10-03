using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Friends;

public class AcceptFriendRequestUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public AcceptFriendRequestUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
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

        if (_currentUser.UserId != friendRequest.TargetUserId)
        {
            throw new ForbiddenException();
        }

        var friendship = Relationship.CreateFriend(friendRequest.UserId, friendRequest.TargetUserId);

        _context.Relationships.Add(friendship);
        _context.Relationships.Remove(friendRequest);

        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyAsync(friendRequest.UserId, "friend.request.accepted", new { RequestId = friendRequest.Id, UserId = _currentUser.UserId }, ct);
    }
}