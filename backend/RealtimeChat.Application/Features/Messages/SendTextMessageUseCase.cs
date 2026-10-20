using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Features.Messages.Services;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages;

public class SendTextMessageUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IMessageService _messageService;

    public SendTextMessageUseCase(IAppDbContext context, ICurrentUser currentUser, IMessageService messageService)
    {
        _context = context;
        _currentUser = currentUser;
        _messageService = messageService;
    }

    public async Task ExecuteAsync(long conversationId, SendTextMessageRequest request, CancellationToken ct)
    {
        var senderId = _currentUser.UserId;

        await _messageService.ValidateCanSendMessageAsync(conversationId, senderId, ct);

        // Lọc ra những metions hợp lệ trong conversations, những mentions không hợp lệ sẽ bị loại bỏ
        var mentionUserIds = await _context.Conversations.FilterMemberIdsAsync(conversationId, request.MentionUserIds, ct);

        // Lấy ReplyMessage nếu request có
        Message? replyToMessage = null;
        if (request.ReplyToMessageId.HasValue)
        {
            replyToMessage = await _context.Messages.FindAsync(request.ReplyToMessageId, ct)
                ?? throw new NotFoundException();
        }

        var message = Message.CreateText(conversationId, senderId, request.Content, mentionUserIds, replyToMessage);

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);

        await _messageService.NotifyMessageCreatedAsync(message, ct);
    }
}

public class SendTextMessageRequest
{
    public required string Content { get; set; }
    public ICollection<long> MentionUserIds { get; set; } = [];
    public long? ReplyToMessageId { get; set; }
}