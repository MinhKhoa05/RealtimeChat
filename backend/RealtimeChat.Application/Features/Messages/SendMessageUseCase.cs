using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages;

public class SendMessageUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public SendMessageUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long conversationId, SendMessageRequest request, CancellationToken ct)
    {
        var senderId = _currentUser.UserId;

        var canAccessConversation = await _context.Conversations
            .FilterAccessible(conversationId, senderId)
            .AnyAsync(ct);

        if (!canAccessConversation)
        {
            throw new ForbiddenException();
        }

        var message = await CreateMessage(conversationId, senderId, request, ct);

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);

        var receiverIds = await _context.ConversationMembers
            .Where(x => x.ConversationId == conversationId)
            .Select(x => x.MemberId)
            .ToListAsync(ct);

        var response = MessageMapper.ToResponse(message);
        await _notifier.NotifyAsync(receiverIds, "message.created", response, ct);
    }

    // Factory tạo message
    private async Task<Message> CreateMessage(long conversationId, long senderId, SendMessageRequest request, CancellationToken ct)
    {
        var type = request.MessageType;

        if (type == MessageType.Text)
        {
            Message? replyToMessage = null;
            if (request.ReplyToMessageId.HasValue)
            {
                replyToMessage = await _context.Messages.FindAsync(request.ReplyToMessageId, ct)
                    ?? throw new NotFoundException();
            }

            var mentionUserIds = await _context.ConversationMembers
                .Where(x => x.ConversationId == conversationId)
                .Where(x => request.MentionUserIds.Contains(x.MemberId))
                .Select(x => x.MemberId)
                .Distinct()
                .ToListAsync(ct);

            return Message.CreateText(conversationId, senderId, request.Content!, mentionUserIds, replyToMessage);
        }

        if (type == MessageType.Media)
        {
            var media = await _context.Medias.FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException();

            return Message.CreateMedia(conversationId, senderId, media);
        }

        if (type == MessageType.Sticker)
        {
            var sticker = await _context.Stickers.FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException();

            return Message.CreateSticker(conversationId, senderId, sticker);
        }

        throw new BadRequestException($"Cannot support for message type {type}");
    }
}

public class SendMessageRequest
{
    public string? Content { get; set; }
    public MessageType MessageType { get; set; }
    public Guid? ReferenceId { get; set; } // Dùng cho loại Sticker / Media
    public ICollection<long> MentionUserIds { get; set; } = [];
    public long? ReplyToMessageId { get; set; }
}