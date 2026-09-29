using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

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

        var user1Id = Math.Min(userId, currentUserId);
        var user2Id = Math.Max(userId, currentUserId);

        var friendship = await _context.Friendships
            .FirstOrDefaultAsync(x=> x.UserId == user1Id && x.FriendId == user2Id, ct);

        if (friendship is null)
        {
            throw new Exception("Not friends.");
        }

        _context.Friendships.Remove(friendship);
        await _context.SaveChangesAsync(ct);
    }
}