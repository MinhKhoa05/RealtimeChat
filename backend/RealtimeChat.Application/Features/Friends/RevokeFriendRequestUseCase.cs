using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

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
            .FriendRequest(requestId)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        if (_currentUser.UserId != friendRequest.UserId)
        {
            throw new ForbiddenException();
        }

        _context.Relationships.Remove(friendRequest);
        await _context.SaveChangesAsync(ct);
    }
}