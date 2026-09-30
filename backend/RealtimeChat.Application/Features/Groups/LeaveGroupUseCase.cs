using Microsoft.EntityFrameworkCore;
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
            .Group(groupId)
            .Include(x => x.Members
                .Where(m => m.MemberId == currentUserId))
            .FirstOrDefaultAsync(ct);
        
        if (group is null)
        {
            throw new Exception("Group not found");
        }

        group.Leave(currentUserId);
        await _context.SaveChangesAsync(ct);
    }
}