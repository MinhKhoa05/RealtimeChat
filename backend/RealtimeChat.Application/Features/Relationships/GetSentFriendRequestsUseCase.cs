using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Relationships;

public class GetSentFriendRequestsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetSentFriendRequestsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<FriendRequestResponse>> ExecuteAsync(CancellationToken ct)
    {
        var friendRequests = await _context.Relationships
            .AsNoTracking()
            .FriendRequests()
            .Where(x => x.UserId == _currentUser.UserId)
            .Select(x => new FriendRequestResponse
            {
                RequestId = x.Id,
                UserId = x.TargetUser.Id,
                Introduction = x.Introduction,
            })
            .ToListAsync(ct);

        return friendRequests;
    }
}