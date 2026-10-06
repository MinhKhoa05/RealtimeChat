using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Application.Features.Call;
using RealtimeChat.Application.Features.Media;

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

        var response = MapToResponse(message);
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

        if (type == MessageType.Call)
        {
            var call = await _context.Calls.FindAsync(request.ReferenceId, ct)
                ?? throw new NotFoundException("Call not found");

            return Message.CreateCall(conversationId, senderId, call);
        }

        if (Message.IsMediaType(type))
        {
            var media = await _context.Medias.FindAsync(request.ReferenceId, ct)
                ?? throw new NotFoundException("Media not found");

            return Message.CreateMedia(conversationId, senderId, media, request.MessageType);
        }

        throw new BadRequestException($"Message type '{type}' is not supported.");
    }

    private static MessageResponse MapToResponse(Message message)
    {
        return new MessageResponse
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Type = message.Type,
            Content = message.Content,
            CreatedAt = message.CreatedAt,

            Media = message.Media is null
                ? null
                : new MediaResponse
                {
                    Id = message.Media.Id,
                    OriginalName = message.Media.OriginalName,
                    ContentType = message.Media.ContentType,
                    Size = message.Media.Size,
                    Url = null!, // Tạm thời để null đi
                },

            Call = message.Call is null
                ? null
                : new CallResponse
                {
                    Id = message.Call.Id,
                    Type = message.Call.Type,
                    Status = message.Call.Status,
                    CreatedAt = message.Call.CreatedAt,
                    Duration = message.Call.Duration,
                }
        };
    }
}

public class SendMessageRequest
{
    public string? Content { get; set; }
    public MessageType MessageType { get; set; }
    public long? ReferenceId { get; set; }
}

public class MessageResponse
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long? SenderId { get; set; }
    public MessageType Type { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public MediaResponse? Media { get; set; }
    public CallResponse? Call { get; set; }
}