using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

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

    public string? DirectKey { get; private set; }

    public ICollection<ConversationMember> Members { get; private set; }
        = new List<ConversationMember>();

    private Conversation() { }

    public static Conversation CreateDirect(long user1Id, long user2Id)
    {
        if (user1Id == user2Id)
        {
            throw new DomainException("Cannot create private conversation with yourself");
        }

        var conversation = new Conversation
        {
            Type = ConversationType.Direct,
            CreatedAt = DateTime.UtcNow,
            DirectKey = GenerateDirectKey(user1Id, user2Id)
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

    public void LeaveGroup(long memberId)
    {
        EnsureGroup();

        var member = FindMember(memberId);
        if (member is null)
        {
            return;
        }

        if (member.Role == ConversationMemberRole.Admin)
        {
            throw new DomainException("Admin cannot leave");
        }

        Members.Remove(member);
    }

    public void KickMember(long actorId, long memberId)
    {
        EnsureGroup();

        if (actorId == memberId)
        {
            throw new DomainException("Cannot kick");
        }

        var actor = FindMember(actorId);
        if (actor is null || actor.Role != ConversationMemberRole.Admin)
        {
            throw new DomainException("Forbidden");
        }

        var targetMember = FindMember(memberId);
        if (targetMember is not null)
        {
            Members.Remove(targetMember);
        }
    }

    public void TransferAdmin(long actorId, long newAdminId)
    {
        EnsureGroup();

        if (actorId == newAdminId) return;

        var actor = FindMember(actorId);
        if (actor is null || actor.Role != ConversationMemberRole.Admin)
            throw new DomainException("Forbidden.");

        var target = FindMember(newAdminId);
        if (target is null)
            throw new DomainException("User is not a member.");

        actor.Role = ConversationMemberRole.Member;
        target.Role = ConversationMemberRole.Admin;
    }

    public void DisbandGroup(long actorId)
    {
        EnsureGroup();

        var actor = FindMember(actorId);
        if (actor is null || actor.Role != ConversationMemberRole.Admin)
            throw new DomainException("Forbidden.");

        DisbandedAt = DateTime.UtcNow;
    }

    private ConversationMember? FindMember(long memberId)
    {
        return Members.FirstOrDefault(x => x.MemberId == memberId);
    }

    private void AddMemberInternal(long memberId, ConversationMemberRole role)
    {
        var exist = FindMember(memberId);
        if (exist is not null)
        {
            throw new DomainException("User is already a member of the conversation.");
        }

        var member = ConversationMember.CreateMember(memberId, role);
        Members.Add(member);
    }

    private void EnsureGroup()
    {
        if (Type != ConversationType.Group)
        {
            throw new DomainException("This operation is only available for group conversations.");
        }
    }

    private static string GenerateDirectKey(long user1Id, long user2Id)
    {
        var min = Math.Min(user1Id, user2Id);
        var max = Math.Max(user1Id, user2Id);

        return $"{min}:{max}";
    }
}