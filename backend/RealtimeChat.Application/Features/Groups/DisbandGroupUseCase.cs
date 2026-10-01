using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Groups;

public class DisbandGroupUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DisbandGroupUseCase(IAppDbContext context, ICurrentUser currentUser)
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
            .Include(x => x.Members.Where(m => m.MemberId == currentUserId))
            .FirstOrDefaultAsync(ct);

        if (group is null)
        {
            throw new Exception("Group not found or forbidden");
        }

        group.DisbandGroup(currentUserId);

        await _context.SaveChangesAsync(ct);
    }
}