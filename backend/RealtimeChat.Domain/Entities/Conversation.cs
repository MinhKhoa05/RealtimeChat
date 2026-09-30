using System.Security.Cryptography;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class Conversation
{
    public long Id { get; private set; }
    public string? Name { get; private set; }
    public ConversationType Type { get; private set; }

    public long? AvatarMediaId { get; private set; }
    public Media? AvatarMedia { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? DisbandedAt { get; private set; }

    public ICollection<ConversationMember> Members { get; private set; }
        = new List<ConversationMember>();

    private Conversation() { }

    public static Conversation CreateDirect(long user1Id, long user2Id)
    {
        var conversation = new Conversation
        {
            Type = ConversationType.Direct,
            CreatedAt = DateTime.UtcNow,
        };

        conversation.AddMemberInternal(user1Id, ConversationMemberRole.Member);
        conversation.AddMemberInternal(user2Id, ConversationMemberRole.Member);

        return conversation;
    }

    public static Conversation CreateGroup(string name, long creatorId)
    {
        var conversation = new Conversation
        {
            Name = name,
            Type = ConversationType.Group,
            CreatedAt = DateTime.UtcNow,
        };

        conversation.AddMemberInternal(creatorId, ConversationMemberRole.Admin);

        return conversation;
    }

    public void AddMember(long memberId)
    {
        EnsureGroup();

        AddMemberInternal(memberId, ConversationMemberRole.Member);
    }

    public void Leave(long memberId)
    {
        EnsureGroup();

        var member = Members.FirstOrDefault(x => x.MemberId == memberId);
        if (member is null)
        {
            return;
        }

        if (member.Role == ConversationMemberRole.Admin)
        {
            throw new Exception("Admin cannot leave");
        }

        Members.Remove(member);
    }

    public void KickMember(long actorId, long memberId)
    {
        EnsureGroup();

        if (actorId == memberId)
        {
            throw new Exception("Cannot kick");
        }

        var actor = Members.FirstOrDefault(x => x.MemberId == actorId);
        if (actor is null || actor.Role != ConversationMemberRole.Admin)
        {
            throw new Exception("Forbidden");
        }

        var targetMember = Members.FirstOrDefault(x => x.MemberId == memberId);
        if (targetMember is not null)
        {
            Members.Remove(targetMember);
        }
    }

    public void TransferAdmin(long actorId, long newAdminId)
    {
        EnsureGroup();

        if (actorId == newAdminId) return;

        var actor = Members.FirstOrDefault(x => x.MemberId == actorId);
        if (actor is null || actor.Role != ConversationMemberRole.Admin)
            throw new Exception("Forbidden.");

        var target = Members.FirstOrDefault(x => x.MemberId == newAdminId);
        if (target is null)
            throw new Exception("User is not a member.");

        actor.Role = ConversationMemberRole.Member;
        target.Role = ConversationMemberRole.Admin;
    }

    private void AddMemberInternal(long memberId, ConversationMemberRole role)
    {
        if (Members.Any(x => x.MemberId == memberId))
        {
            throw new Exception("User is already a member of the conversation.");
        }

        var member = ConversationMember.CreateMember(memberId, role);
        Members.Add(member);
    }

    private void EnsureGroup()
    {
        if (Type == ConversationType.Direct)
        {
            throw new Exception("Not groups");
        }
    }
}