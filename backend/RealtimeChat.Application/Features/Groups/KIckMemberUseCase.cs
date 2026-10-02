using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
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
            .FilterAccessibleGroup(groupId, currentUserId)
            .WithMembers(currentUserId, userId) // Lấy member liên quan để thực hiện hành động
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        if (!group.IsAdmin(currentUserId))
        {
            throw new ForbiddenException();
        }

        group.RemoveMember(userId);
        await _context.SaveChangesAsync(ct);
    }
}