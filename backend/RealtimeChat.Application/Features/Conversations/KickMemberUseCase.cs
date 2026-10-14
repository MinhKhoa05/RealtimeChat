using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Conversations;

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
        var actorId = _currentUser.UserId;

        var group = await _context.Conversations
            .FilterAccessibleGroup(groupId, actorId)
            .WithMembers(actorId, userId) // Lấy member liên quan để thực hiện hành động
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        group.KickMember(actorId, userId);
        await _context.SaveChangesAsync(ct);
    }
}