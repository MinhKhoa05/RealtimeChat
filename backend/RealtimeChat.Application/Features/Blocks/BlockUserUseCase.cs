using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Blocks;

public class BlockUserUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public BlockUserUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        if (currentUserId == userId)
        {
            throw new Exception("Cannot block to yourself");
        }

        var targetExists = await _context.Users.AnyAsync(x => x.Id == userId, ct);
        if (!targetExists)
        {
            throw new Exception("User not found");
        }

        var exists = await _context.Relationships
            .AnyAsync(x => x.Type == Domain.Enums.RelationshipType.Block && x.UserId == currentUserId && x.TargetUserId == userId, ct);

        if (exists)
        {
            throw new Exception("Already block");
        }

        var userBlock = new Relationship
        {
            UserId = currentUserId,
            TargetUserId = userId,
            CreatedAt = DateTime.UtcNow,
            Type = Domain.Enums.RelationshipType.Block
        };

        _context.Relationships.Add(userBlock);

        var user1Id = Math.Min(userId, currentUserId);
        var user2Id = Math.Max(userId, currentUserId);

        var friendship = await _context.Relationships
            .FirstOrDefaultAsync(x => x.Type == Domain.Enums.RelationshipType.Friend && x.UserId == user1Id && x.TargetUserId == user2Id, ct);
        
        if (friendship is not null)
        {
            _context.Relationships.Remove(friendship);
        }

        var friendRequests = await _context.Relationships
            .Where(x =>
                x.Type == Domain.Enums.RelationshipType.FriendRequest &&
                ((x.UserId == currentUserId && x.TargetUserId == userId) ||
                (x.UserId == userId && x.TargetUserId == currentUserId)))
            .ToListAsync(ct);

        _context.Relationships.RemoveRange(friendRequests);

        await _context.SaveChangesAsync(ct);
    }
}