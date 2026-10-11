using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Relationships;

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
            .FriendRequests()
            .Where(x => x.TargetUserId == _currentUser.UserId)
            .Select(x => new FriendRequestResponse
            {
                RequestId = x.Id,
                UserId = x.User.Id,
                Introduction = x.Introduction,
            })
            .ToListAsync(ct);

        return friendRequests;
    }
}

public class FriendRequestResponse
{
    public long RequestId { get; set; }
    public long UserId { get; set; }
    public string? Introduction { get; set; }
}