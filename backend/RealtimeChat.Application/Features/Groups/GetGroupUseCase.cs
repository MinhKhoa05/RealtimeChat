using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Groups;

public class GetGroupUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetGroupUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<GroupResponse> ExecuteAsync(long groupId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var group = await _context.Conversations
            .FilterAccessibleGroup(groupId, currentUserId)
            .Select(x => new GroupResponse
            {
                GroupId = x.Id,
                Name = x.Name!,
                AvatarUrl = null,
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        return group;
    }
}

public class GroupResponse
{
    public long GroupId {get; set; }
    public string Name { get; set; } = null!;
    public string? AvatarUrl { get; set; }
}