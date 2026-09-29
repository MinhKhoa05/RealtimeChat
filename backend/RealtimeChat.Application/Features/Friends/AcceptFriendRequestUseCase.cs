using RealtimeChat.Application.Interfaces;
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
        var friendRequest = await _context.FriendRequests.FindAsync(requestId)
            ?? throw new Exception("Request not found");

        if (_currentUser.UserId != friendRequest.ReceiverId)
        {
            throw new Exception("Cannot accept");
        }
        
        var user1Id = Math.Min(friendRequest.SenderId, friendRequest.ReceiverId);
        var user2Id = Math.Max(friendRequest.SenderId, friendRequest.ReceiverId);

        var friendship = new Friendship
        {
            UserId = user1Id,
            FriendId = user2Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Friendships.Add(friendship);
        _context.FriendRequests.Remove(friendRequest);
        await _context.SaveChangesAsync(ct);
    }
}