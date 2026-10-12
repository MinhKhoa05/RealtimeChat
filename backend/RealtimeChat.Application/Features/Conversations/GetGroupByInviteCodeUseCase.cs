using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Conversations;

public class GetGroupByInviteCodeUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetGroupByInviteCodeUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<GroupInviteResponse> ExecuteAsync(Guid inviteCode, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var groupInvite = await _context.Conversations
            .AsNoTracking()
            .Where(x => x.InviteCode == inviteCode)
            .Active()
            .Select(x => new GroupInviteResponse
            {
                AvatarUrl = null,
                Name = x.Name,
                IsMember = x.Members.Any(m => m.MemberId == currentUserId),
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        return groupInvite;
    }
}

public class GroupInviteResponse
{
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsMember { get; set; }
}