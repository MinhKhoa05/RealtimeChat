using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Friends;

public class RevokeFriendRequestUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RevokeFriendRequestUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long requestId, CancellationToken ct)
    {
        var friendRequest = await _context.Relationships
            .FirstOrDefaultAsync(x => x.Type == Domain.Enums.RelationshipType.FriendRequest && x.Id == requestId, ct)
            ?? throw new Exception("Request not found");

        if (_currentUser.UserId != friendRequest.UserId)
        {
            throw new Exception("Cannot revoke");
        }

        _context.Relationships.Remove(friendRequest);
        await _context.SaveChangesAsync(ct);
    }
}