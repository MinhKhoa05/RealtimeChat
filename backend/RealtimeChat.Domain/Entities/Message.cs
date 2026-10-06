using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Message
{
    public long Id { get; private set; }
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

    public DateTime CreatedAt { get; private set; }
    public DateTime? RecalledAt { get; private set; }

    private static readonly TimeSpan RecallWindow = TimeSpan.FromMinutes(15);

    private Message() { }

    public static Message CreateText(long conversationId, long senderId, string content)
    {
        return new Message
        {
            ConversationId = conversationId,
            Type = MessageType.Text,
            SenderId = senderId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
        };
    }

    // Tạo MediaMessage hoặc CallMessage
    public static Message CreateReference(long conversationId, MessageType type, long senderId, long referenceId)
    {
        var message = new Message
        {
            ConversationId = conversationId,
            Type = type,
            SenderId = senderId,
            CreatedAt = DateTime.UtcNow,
        };

        if (IsMediaType(type))
        {
            message.MediaId = referenceId;
        }
        else if (type == MessageType.Call)
        {
            message.CallId = referenceId;
        }
        else
        {
            throw new DomainException("Message type does not support reference.");
        }

        return message;
    }

    public static Message CreateSystem(long conversationId, string content)
    {
        return new Message
        {
            ConversationId = conversationId,
            Type = MessageType.System,
            Content = content,
            CreatedAt = DateTime.UtcNow,
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

    private static bool IsMediaType(MessageType type)
    {
        return type == MessageType.Image ||
            type == MessageType.Video ||
            type == MessageType.Audio ||
            type == MessageType.File;
    }
}