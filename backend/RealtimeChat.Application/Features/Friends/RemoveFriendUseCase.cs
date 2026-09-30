using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Friends;

public class RemoveFriendUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RemoveFriendUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        if (currentUserId == userId)
        {
            throw new Exception("Cannot remove friend to yourself.");
        }

        var friendship = await _context.Relationships
            .Friend(currentUserId, userId)
            .FirstOrDefaultAsync(ct)
            ?? throw new Exception("Not friends");

        _context.Relationships.Remove(friendship);
        await _context.SaveChangesAsync(ct);
    }
}