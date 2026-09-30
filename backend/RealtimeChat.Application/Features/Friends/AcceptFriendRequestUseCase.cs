using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Friends;

public class AcceptFriendRequestUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AcceptFriendRequestUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long requestId, CancellationToken ct)
    {
        var friendRequest = await _context.Relationships
            .FriendRequests()
            .FirstOrDefaultAsync(x => x.Id == requestId, ct)
            ?? throw new Exception("Request not found");

        if (_currentUser.UserId != friendRequest.TargetUserId)
        {
            throw new Exception("Cannot accept");
        }

        var friendship = Relationship.CreateFriend(friendRequest.UserId, friendRequest.TargetUserId);

        _context.Relationships.Remove(friendRequest);
        _context.Relationships.Add(friendship);

        await _context.SaveChangesAsync(ct);
    }
}