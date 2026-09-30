using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

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
            .FirstOrDefaultAsync(x => x.Id == requestId && x.Type == RelationshipType.FriendRequest, ct)
            ?? throw new Exception("Request not found");

        if (_currentUser.UserId != friendRequest.TargetUserId)
        {
            throw new Exception("Cannot accept");
        }
        
        var user1Id = Math.Min(friendRequest.UserId, friendRequest.TargetUserId);
        var user2Id = Math.Max(friendRequest.UserId, friendRequest.TargetUserId);

        var friendship = new Relationship
        {
            UserId = user1Id,
            TargetUserId = user2Id,
            Type = RelationshipType.Friend,
            CreatedAt = DateTime.UtcNow
        };

        _context.Relationships.Remove(friendRequest);
        _context.Relationships.Add(friendship);

        await _context.SaveChangesAsync(ct);
    }
}