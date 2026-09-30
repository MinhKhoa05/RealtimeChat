using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
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

        var exists = await _context.Relationships.Block(currentUserId, userId).AnyAsync(ct);

        if (exists)
        {
            throw new Exception("Already block");
        }

        var blocks = Relationship.CreateBlock(currentUserId, userId);
        _context.Relationships.Add(blocks);

        var friendship = await _context.Relationships
            .Friend(currentUserId, userId)
            .FirstOrDefaultAsync(ct);
            
        if (friendship is not null)
        {
            _context.Relationships.Remove(friendship);
        }

        var friendRequests = await _context.Relationships
            .FriendRequests()
            .Between(currentUserId, userId)
            .ToListAsync(ct);

        _context.Relationships.RemoveRange(friendRequests);

        await _context.SaveChangesAsync(ct);
    }
}