using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class ConversationMember
{
    public long ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    public long MemberId { get; private set; }
    public User Member { get; private set; } = null!;

    public ConversationMemberRole Role { get; set; }

    public long? LastReadMessageId { get; set; }
    public Message? LastReadMessage { get; set; }

    public bool IsPinned { get; set; }
    public DateTime JoinedAt { get; set; }

    private ConversationMember() { }

    public static ConversationMember CreateMember(long userId, ConversationMemberRole role)
    {
        return new ConversationMember
        {
            MemberId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow,
        };
    }

    public static ConversationMember CreateMember(long converstationId, long userId, ConversationMemberRole role)
    {
        return new ConversationMember
        {
            ConversationId = converstationId,
            MemberId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow,
        };
    }
}