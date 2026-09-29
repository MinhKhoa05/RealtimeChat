using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

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
        var friendRequests = await _context.FriendRequests
            .AsNoTracking()
            .Where(x => x.ReceiverId == _currentUser.UserId)
            .Select(x => new FriendRequestResponse
            {
                RequestId = x.Id,
                UserId = x.Receiver.Id,
                Introduction = x.Introduction,
                Name = x.Receiver.Name,
                AvatarUrl = null
            })
            .ToListAsync(ct);

        return friendRequests;
    }
}