using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

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
        var message = await _context.Messages
            .Where(x => x.Id == messageId)
            .Include(x => x.Mentions) // Để domain xóa lun Mentions
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException();

        message.Recall(_currentUser.UserId, DateTime.UtcNow);
        await _context.SaveChangesAsync(ct);

        var memberIds = await _context.Conversations
            .GetMembers([message.ConversationId])            
            .Select(x => x.MemberId)
            .ToListAsync(ct);

        await _notifier.NotifyAsync(memberIds, "message.recall", new { MessageId = messageId }, ct);
    }
}