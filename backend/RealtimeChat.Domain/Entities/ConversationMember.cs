using System.Runtime.CompilerServices;
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

    public bool IsPinned { get; private set; }

    public long? InvitedById { get; private set; } // Null nếu tham gia qua link nhóm.
    public User? InvitedBy { get; private set; }

    private ConversationMember() { }

    internal static ConversationMember CreateMember(long userId, long? invitedById = null)
    {
        return new ConversationMember
        {
            MemberId = userId,
            Role = ConversationMemberRole.Member,
            InvitedById = invitedById,
        };
    }

    internal static ConversationMember CreateAdmin(long userId)
    {
        return new ConversationMember
        {
            MemberId = userId,
            Role = ConversationMemberRole.Admin,
        };
    }

    internal bool IsAdmin() => Role == ConversationMemberRole.Admin;
    internal void SetRole(ConversationMemberRole role) => Role = role;

    internal void SetPinned(bool isPinned) => IsPinned = isPinned;

    internal bool MarkAsRead(long messageId)
    {
        if (LastReadMessageId.HasValue && messageId < LastReadMessageId)
            return false;

        LastReadMessageId = messageId;
        return true;
    }
}