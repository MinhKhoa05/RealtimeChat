using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class Message
{
    public long Id { get; set; }
    public MessageType Type { get; set; }
    public string? Content { get; set; }

    public long ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public long? SenderId { get; set; }
    public User? Sender { get; set; }

    public long? MediaId { get; set; }
    public Media? Media { get; set; }

    public long? CallId { get; set; }
    public Call? Call { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? RecalledAt { get; set; }
}