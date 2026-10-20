using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Factories;

namespace RealtimeChat.Application.Features.Conversations;

public class DisbandGroupUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DisbandGroupUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long conversationId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var conversation = await _context.Conversations.GetWithMembersAsync(conversationId, [actorId], ct)
            ?? throw new NotFoundException();

        conversation.DisbandGroup(actorId, DateTime.UtcNow);

        var message = SystemMessageFactory.CreateGroupDisbanded(conversationId, actorId);
        _context.Messages.Add(message);

        await _context.SaveChangesAsync(ct);
    }
}