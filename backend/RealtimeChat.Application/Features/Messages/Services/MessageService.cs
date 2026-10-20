using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages.Services;

public class MessageService : IMessageService
{
    private readonly IAppDbContext _context;
    private readonly IClientNotifier _notifier;

    public MessageService(IAppDbContext context, IClientNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task ValidateCanSendMessageAsync(long conversationId, long senderId, CancellationToken ct)
    {
        var conversation = await _context.Conversations.GetWithMembersAsync(conversationId, [senderId], ct)
            ?? throw new NotFoundException();

        conversation.EnsureCanSendMessage(senderId);
    }

    public async Task NotifyMessageCreatedAsync(Message message, CancellationToken ct)
    {
        var memberIds = await _context.Conversations
            .GetMembers([message.ConversationId])
            .Select(x => x.MemberId)
            .ToListAsync(ct);

        await _notifier.NotifyAsync(memberIds, "message.created", MessageMapper.ToResponse(message), ct);
    }
}