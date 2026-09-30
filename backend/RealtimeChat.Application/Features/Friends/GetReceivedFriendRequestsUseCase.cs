using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Friends;

public class GetReceivedFriendRequestsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetReceivedFriendRequestsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<FriendRequestResponse>> ExecuteAsync(CancellationToken ct)
    {
        var friendRequests = await _context.Relationships
            .AsNoTracking()
            .Where(x => x.Type == RelationshipType.FriendRequest && x.TargetUserId == _currentUser.UserId)
            .Select(x => new FriendRequestResponse
            {
                RequestId = x.Id,
                UserId = x.User.Id,
                Introduction = x.Introduction,
                Name = x.User.Name,
                AvatarUrl = null
            })
            .ToListAsync(ct);

        return friendRequests;
    }
}