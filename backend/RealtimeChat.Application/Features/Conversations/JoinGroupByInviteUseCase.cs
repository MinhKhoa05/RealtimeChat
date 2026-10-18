using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Conversations;

public class JoinGroupByInviteUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public JoinGroupByInviteUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(Guid inviteCode, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var conversation = await _context.Conversations
            .Where(x => x.InviteCode == inviteCode)
            .IncludeMembers(actorId)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        conversation.JoinByInviteKey(actorId);
        await _context.SaveChangesAsync(ct);
    }
}