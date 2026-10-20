using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Factories;

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

        var conversation = await _context.Conversations.GetWithMembersByInviteCodeAsync(inviteCode, [actorId], ct)
            ?? throw new NotFoundException();

        conversation.JoinByInviteKey(actorId);

         var message = SystemMessageFactory.CreateMemberJoined(conversation.Id, actorId);
        _context.Messages.Add(message);

        await _context.SaveChangesAsync(ct);
    }
}