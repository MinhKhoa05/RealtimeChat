using RealtimeChat.Application.Features.Calls;
using RealtimeChat.Application.Features.Media;
using RealtimeChat.Application.Features.Stickers;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Messages;

public static class MessageMapper
{
    public static MessageResponse ToResponse(Message message)
    {
        var isRecalled = message.RecalledAt.HasValue;

        if (isRecalled)
        {
            return new MessageResponse
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                Type = message.Type,
                CreatedAt = message.CreatedAt,
                IsRecalled = isRecalled,
            };
        }

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
                : MediaMapper.ToResponse(message.Media),

            Sticker = message.Sticker is null
                ? null
                : StickerMapper.ToResponse(message.Sticker),

            Call = message.Call is null
                ? null
                : new CallResponse
                {
                    Id = message.Call.Id,
                    Type = message.Call.Type,
                    Status = message.Call.Status,
                    CreatedAt = message.Call.CreatedAt,
                    Duration = message.Call.Duration()
                },

            Mentions = message.Mentions
                .Select(x => new MentionDto
                {
                    UserId = x.UserId,
                    Start = x.Start,
                    Length = x.Length
                })
                .ToList(),

            ReplyToMessage = message.ReplyToMessage is null
                ? null
                : new MessageResponse
                {
                    Id = message.ReplyToMessage.Id,
                    SenderId = message.ReplyToMessage.SenderId,
                    Type = message.ReplyToMessage.Type,
                    IsRecalled = message.ReplyToMessage.RecalledAt.HasValue,
                },

            IsRecalled = false
        };
    }
}