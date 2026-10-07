using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class ConversationMember
{
    public long ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    public long MemberId { get; private set; }
    public User Member { get; private set; } = null!;

    public ConversationMemberRole Role { get; private set; }

    public long? LastReadMessageId { get; private set; }
    public Message? LastReadMessage { get; private set; }

    public bool IsPinned { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private ConversationMember() { }

    internal static ConversationMember Create(long userId, ConversationMemberRole role)
    {
        return new ConversationMember
        {
            MemberId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow,
        };
    }

    internal void SetRole(ConversationMemberRole role)
    {
        Role = role;
    }

    public void Pin()
    {
        IsPinned = true;
    }

    public void Unpin()
    {
        IsPinned = false;
    }

    public void MarkAsRead(long messageId)
    {
        LastReadMessageId = messageId;
    }
}