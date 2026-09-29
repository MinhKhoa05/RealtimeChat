using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Friends;

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
        var friendRequests = await _context.FriendRequests
            .AsNoTracking()
            .Where(x => x.SenderId == _currentUser.UserId)
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

public class FriendRequestResponse
{
    public long RequestId { get; set; }
    public long UserId { get; set; }
    public string? Introduction { get; set; }
    public string Name { get; set; } = null!;
    public string? AvatarUrl { get; set; }
}