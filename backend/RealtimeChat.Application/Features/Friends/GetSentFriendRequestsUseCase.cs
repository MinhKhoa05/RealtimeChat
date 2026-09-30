using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Enums;

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
        var friendRequests = await _context.Relationships
            .AsNoTracking()
            .Where(x => x.Type == RelationshipType.FriendRequest && x.UserId == _currentUser.UserId)
            .Select(x => new FriendRequestResponse
            {
                RequestId = x.Id,
                UserId = x.TargetUser.Id,
                Introduction = x.Introduction,
                Name = x.TargetUser.Name,
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