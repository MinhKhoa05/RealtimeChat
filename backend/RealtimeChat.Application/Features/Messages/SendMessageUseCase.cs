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
            return Message.CreateText(conversationId, senderId, request.Content!);
        }

        if (type == MessageType.Sticker)
        {
            var sticker = await _context.Stickers.FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException("Sticker not found");

            return Message.CreateSticker(conversationId, senderId, sticker);
        }

        if (Message.IsMediaType(type))
        {
            var media = await _context.Medias.FirstOrDefaultAsync(x => x.PublicId == request.ReferenceId, ct)
                ?? throw new NotFoundException("Media not found");

            return Message.CreateMedia(conversationId, senderId, media, request.MessageType);
        }

        throw new BadRequestException($"Message type '{type}' is not supported.");
    }
}

public class SendMessageRequest
{
    public string? Content { get; set; }
    public MessageType MessageType { get; set; }
    public Guid? ReferenceId { get; set; }
}