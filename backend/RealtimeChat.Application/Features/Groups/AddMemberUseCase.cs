using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Groups;

public class AddMemberUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AddMemberUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long groupId, long userId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        if (userId == currentUserId)
        {
            throw new Exception("Cannot add yourself.");
        }

        var group = await _context.Conversations
            .Active()
            .Group(groupId)
            .AccessibleBy(currentUserId)
            .Include(x => x.Members
                .Where(m => m.MemberId == userId))
            .FirstOrDefaultAsync(ct);

        if (group is null)
        {
            throw new Exception("Group not founds or fobidden");
        }

        // UserId đã trong group thì không làm gì nữa
        if (group.Members.Any())
        {
            return;
        }
        
        var isFriend = await _context.Relationships
            .Friends()
            .Between(userId, currentUserId)
            .AnyAsync(ct);

        if (!isFriend)
        {
            throw new Exception("Must be friends");
        }

        group.AddMember(userId);
        await _context.SaveChangesAsync(ct);
    }
}