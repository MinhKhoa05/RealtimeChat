using RealtimeChat.Application.Features.Calls;
using RealtimeChat.Application.Features.Media;
using RealtimeChat.Application.Features.Stickers;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Messages;

public static class MessageMapper
{
    public static MessageResponse ToResponse(Message message)
    {
        var isRecalled = message.RecalledAt.HasValue;

        return new MessageResponse
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Type = message.Type,
            Content = isRecalled ? null : message.Content,
            CreatedAt = message.CreatedAt,
            IsRecalled = isRecalled,

            Payload = isRecalled ? null : MapPayload(message),

            MentionUserIds = message.Mentions.Select(x => x.UserId).ToList(),

            ReplyToMessage = message.ReplyToMessage is null
                ? null
                : ToReplyPreview(message.ReplyToMessage)
        };
    }

    private static object? MapPayload(Message message) =>
        message.Type switch
        {
            MessageType.Media when message.Media is not null =>
                MediaMapper.ToResponse(message.Media),

            MessageType.Sticker when message.Sticker is not null =>
                StickerMapper.ToResponse(message.Sticker),

            MessageType.Call when message.Call is not null =>
                new CallResponse
                {
                    Id = message.Call.Id,
                    Type = message.Call.Type,
                    Status = message.Call.Status,
                    CreatedAt = message.Call.CreatedAt,
                    Duration = message.Call.Duration
                },

            _ => null
        };

    private static MessageResponse ToReplyPreview(Message message) =>
        new()
        {
            Id = message.Id,
            SenderId = message.SenderId,
            Type = message.Type,
            Content = message.RecalledAt.HasValue ? null : message.Content,
            IsRecalled = message.RecalledAt.HasValue,

            MentionUserIds = message.Mentions.Select(x => x.UserId).ToList(),
        };
}