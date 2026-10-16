using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class ConversationMember : BaseEntity
{
    public long ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    public long MemberId { get; private set; }
    public User Member { get; private set; } = null!;

    public ConversationMemberRole Role { get; private set; }

    public long? LastReadMessageId { get; private set; }
    public Message? LastReadMessage { get; private set; }

    public bool IsPinned { get; private set; }

    public long? InvitedById { get; private set; } // Null nếu tham gia qua link nhóm.
    public User? InvitedBy { get; private set; }

    private ConversationMember() { }

    public static ConversationMember CreateMember(long userId, long? invitedById = null)
    {
        return new ConversationMember
        {
            MemberId = userId,
            Role = ConversationMemberRole.Member,
            InvitedById = invitedById,
        };
    }

    public static ConversationMember CreateAdmin(long userId)
    {
        return new ConversationMember
        {
            MemberId = userId,
            Role = ConversationMemberRole.Admin,
        };
    }

    public bool IsAdmin() => Role == ConversationMemberRole.Admin;

    public void SetRole(ConversationMemberRole role)
    {
        Role = role;
    }

    public void SetPin(bool isPinned)
    {
        IsPinned = isPinned;
    }

    public void MarkAsRead(long messageId)
    {
        // Bỏ qua nếu message đã đọc có ID nhỏ hơn message cuối cùng đã đọc.
        if (messageId < LastReadMessageId)
        {
            return;
        }

        LastReadMessageId = messageId;
    }
}