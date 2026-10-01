using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Groups;

public class LeaveGroupUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public LeaveGroupUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long groupId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var group = await _context.Conversations
            .Active()
            .Group(groupId)
            .AccessibleBy(currentUserId)
            .Include(x => x.Members
                .Where(m => m.MemberId == currentUserId))
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();
        
        group.LeaveGroup(currentUserId);
        await _context.SaveChangesAsync(ct);
    }
}