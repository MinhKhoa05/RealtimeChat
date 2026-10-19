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

    public async Task ExecuteAsync(long conversationId, long userId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var conversation = await _context.Conversations.GetWithMembersAsync(conversationId, [actorId, userId], ct)
            ?? throw new NotFoundException();

        conversation.KickMember(actorId, userId);
        await _context.SaveChangesAsync(ct);
    }
}