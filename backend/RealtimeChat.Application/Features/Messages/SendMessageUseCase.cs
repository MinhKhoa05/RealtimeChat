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

        if (request.MessageType == MessageType.System)
        {
            throw new BadRequestException("Cannot send System Message.");
        }

        if (request.MessageType != MessageType.Text && !request.ReferenceId.HasValue)
        {
            throw new BadRequestException("ReferenceId is required.");
        }

        var message = request.MessageType switch
        {
            MessageType.Text => Message.CreateTextMessage(conversationId, senderId, request.Content!),

            // CreateReferenceMessage() đã kiểm tra và đảm bảo MessageType hợp lệ phù hợp cho ReferenceMessage
            _ => Message.CreateReferenceMessage(conversationId, request.MessageType, senderId, request.ReferenceId!.Value),
        };

        Media? media = null;
        Call? call = null;

        // Check các Reference có tồn tại không
        if (message.IsCallMessage())
        {
            call = await _context.Calls.FirstOrDefaultAsync(x => x.Id == message.CallId, ct)
                ?? throw new NotFoundException("Call not found.");
        }
        else if (message.IsMediaMessage())
        {
            media = await _context.Medias.FirstOrDefaultAsync(x => x.Id == message.MediaId, ct)
                ?? throw new NotFoundException("Media not found");
        }

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(ct);

        var receiverIds = await _context.ConversationMembers
            .Where(x => x.ConversationId == conversationId)
            .Select(x => x.MemberId)
            .ToListAsync(ct);

        var response = MapToResponse(message, media, call);

        await _notifier.NotifyAsync(receiverIds, "message.created", response, ct);
    }

    private static MessageResponse MapToResponse(Message message, Media? media, Call? call)
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
                    Url = null! // Tạm thời để null đi
                },

            Call = message.Call is null
                ? null
                : new CallResponse
                {
                    Id = message.Call.Id,
                    Type = message.Call.Type,
                    Status = message.Call.Status,
                    StartedAt = message.Call.StartedAt,
                    EndedAt = message.Call.EndedAt
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
    public long Id { get; init; }
    public long ConversationId { get; init; }
    public long? SenderId { get; init; }
    public MessageType Type { get; init; }
    public string? Content { get; init; }
    public DateTime CreatedAt { get; init; }

    public MediaResponse? Media { get; init; }
    public CallResponse? Call { get; init; }
}

// Tạm thời để 2 cái Media với Call Response này ở đây, mốt refactor sau
public class MediaResponse
{
    public long Id { get; init; }
    public string OriginalName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }
    public string Url { get; init; } = null!;
}

public class CallResponse
{
    public long Id { get; init; }
    public CallType Type { get; init; }
    public CallStatus Status { get; init; }

    public DateTime StartedAt { get; init; }
    public DateTime? EndedAt { get; init; }
}