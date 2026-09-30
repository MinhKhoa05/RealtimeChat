using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Groups;

public class GetMyGroupsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyGroupsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<GroupResponse>> ExecuteAsync(CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var groups = await _context.Conversations
            .AsNoTracking()
            .Active()
            .Groups()
            .Where(x => x.Members.Any(m => m.MemberId == currentUserId))
            .Select(x => new GroupResponse
            {
                GroupId = x.Id,
                Name = x.Name!,
                AvatarUrl = null,
            })
            .ToListAsync(ct);

        return groups;
    }
}