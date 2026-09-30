using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Friends;

public class GetFriendsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetFriendsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<FriendResponse>> ExecuteAsync(CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var friends = await _context.Relationships
            .AsNoTracking()
            .Friends()
            .Where(x => x.UserId == currentUserId || x.TargetUserId == currentUserId)
            .Select(x => x.UserId == currentUserId ? x.TargetUser : x.User)
            .Select(x => new FriendResponse
            {
                UserId = x.Id,
                Name = x.Name,
                AvatarUrl = null,
            })
            .ToListAsync(ct);

        return friends;
    }

}

public class FriendResponse
{
    public long UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? AvatarUrl { get; set; }
}