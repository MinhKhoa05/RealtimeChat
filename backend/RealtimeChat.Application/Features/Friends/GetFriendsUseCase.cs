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

    public async Task<List<long>> ExecuteAsync(CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var friendIds = await _context.Relationships
            .AsNoTracking()
            .Friends()
            .OfUsers(currentUserId)
            .Select(x => x.UserId == currentUserId ? x.TargetUserId : x.UserId)
            .ToListAsync(ct);

        return friendIds;
    }
}