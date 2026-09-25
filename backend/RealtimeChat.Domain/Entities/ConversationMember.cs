using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class ConversationMember
{
    public long ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public long MemberId { get; set; }
    public User Member { get; set; } = null!;

    public ConversationMemberRole Role { get; set; }

    public long? LastReadMessageId { get; set; }
    public Message? LastReadMessage { get; set; }

    public bool IsPinned { get; set; }
    public DateTime JoinedAt { get; set; }
}