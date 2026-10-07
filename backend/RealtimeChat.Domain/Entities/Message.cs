using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Message : BaseEntity
{
    public MessageType Type { get; private set; }
    public string? Content { get; private set; }

    public long ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    public long? SenderId { get; private set; }
    public User? Sender { get; private set; }

    public long? MediaId { get; private set; }
    public Media? Media { get; private set; }

    public long? CallId { get; private set; }
    public Call? Call { get; private set; }

    public long? StickerId { get; private set; }
    public Sticker? Sticker { get; private set; }

    public DateTime? RecalledAt { get; private set; }

    private static readonly TimeSpan RecallWindow = TimeSpan.FromMinutes(15);

    private Message() { }

    public static Message CreateText(long conversationId, long senderId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainException("Message content cannot be empty.");
        }

        return new Message
        {
            ConversationId = conversationId,
            Type = MessageType.Text,
            SenderId = senderId,
            Content = content,
        };
    }

    public static Message CreateSticker(long conversationId, long senderId, Sticker sticker)
    {
        return new Message
        {
            ConversationId = conversationId,
            Type = MessageType.Sticker,
            SenderId = senderId,
            StickerId = sticker.Id,
            Sticker = sticker,
        };
    }

    public static Message CreateCall(long conversationId, long senderId, Call call)
    {
        return new Message
        {
            ConversationId = conversationId,
            Type = MessageType.Call,
            SenderId = senderId,
            CallId = call.Id,
            Call = call,
        };
    }

    public static Message CreateMedia(long conversationId, long senderId, Media media, MessageType type)
    {
        if (!IsMediaType(type))
        {
            throw new DomainException("Message type must be a media type.");
        }

        return new Message
        {
            ConversationId = conversationId,
            Type = type,
            SenderId = senderId,
            MediaId = media.Id,
            Media = media,
        };
    }

    public static Message CreateSystem(long conversationId, string content)
    {
        return new Message
        {
            ConversationId = conversationId,
            Type = MessageType.System,
            Content = content,
        };
    }

    public void Recall()
    {
        if (RecalledAt.HasValue)
        {
            throw new DomainException("Message has already been recalled.");
        }

        if (DateTime.UtcNow > CreatedAt.Add(RecallWindow))
        {
            throw new DomainException("Message can no longer be recalled.");
        }

        RecalledAt = DateTime.UtcNow;
    }

    public bool IsCallMessage() => Type == MessageType.Call && CallId != null;

    public bool IsMediaMessage() => IsMediaType(Type) && MediaId != null;

    public static bool IsMediaType(MessageType type)
    {
        return type == MessageType.Image ||
            type == MessageType.Video ||
            type == MessageType.Audio ||
            type == MessageType.File;
    }
}