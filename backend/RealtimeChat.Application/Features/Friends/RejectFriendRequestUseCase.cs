using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Friends;

public class RejectFriendRequestUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RejectFriendRequestUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long requestId, CancellationToken ct)
    {
        var friendRequest = await _context.Relationships
            .FriendRequest(requestId)
            .FirstOrDefaultAsync(ct)
            ?? throw new Exception("Request not found");

        if (_currentUser.UserId != friendRequest.TargetUserId)
        {
            throw new Exception("Cannot reject");
        }

        _context.Relationships.Remove(friendRequest);
        await _context.SaveChangesAsync(ct);
    }
}