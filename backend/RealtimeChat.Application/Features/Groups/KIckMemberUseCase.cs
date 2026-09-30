using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Groups;

public class KickMemberUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public KickMemberUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long groupId, long userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var group = await _context.Conversations
            .Group(groupId)
            .Include(x => x.Members
                .Where(m => m.MemberId == userId || m.MemberId == currentUserId))
            .FirstOrDefaultAsync(ct);
        
        if (group is null)
        {
            throw new Exception("Group not found");
        }

        group.KickMember(currentUserId, userId);
        await _context.SaveChangesAsync(ct);
    }
}