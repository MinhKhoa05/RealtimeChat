using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Messages;

public class RecallMessageUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public RecallMessageUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long messageId, CancellationToken ct)
    {
        var message = await _context.Messages.FindAsync(messageId, ct)
            ?? throw new NotFoundException();

        if (message.SenderId != _currentUser.UserId)
        {
            throw new ForbiddenException();
        }

        message.Recall();
        await _context.SaveChangesAsync(ct);

        var memberIds = await _context.ConversationMembers
            .AsNoTracking()
            .Where(x => x.ConversationId == message.ConversationId)
            .Select(x => x.MemberId)
            .ToListAsync(ct);

        await _notifier.NotifyAsync(memberIds, "message.recall", new { MessageId = messageId }, ct);
    }
}