using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Conversation : BaseEntity
{
    public string? Name { get; private set; }
    public ConversationType Type { get; private set; }

    public long? AvatarMediaId { get; private set; }
    public Media? AvatarMedia { get; private set; }

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
        };

        conversation.AddMemberInternal(creatorId, ConversationMemberRole.Admin);

        return conversation;
    }

    public void AddMember(long memberId)
    {
        EnsureGroup();

        AddMemberInternal(memberId, ConversationMemberRole.Member);
    }

    public void RemoveMember(long memberId)
    {
        EnsureGroup();

        var member = FindMember(memberId)
            ?? throw new DomainException("User is not a member.");

        if (member.Role == ConversationMemberRole.Admin)
        {
            throw new DomainException("Cannot remove the admin.");
        }

        Members.Remove(member);
    }

    public void TransferAdmin(long newAdminId)
    {
        EnsureGroup();

        var currentAdmin = Members.FirstOrDefault(x => x.Role == ConversationMemberRole.Admin)
            ?? throw new DomainException("Group has no admin.");

        var target = FindMember(newAdminId)
            ?? throw new DomainException("User is not a member.");

        if (currentAdmin.MemberId == newAdminId)
        {
            return;
        }

        currentAdmin.SetRole(ConversationMemberRole.Admin);
        target.SetRole(ConversationMemberRole.Member);
    }

    public void DisbandGroup()
    {
        EnsureGroup();

        if (DisbandedAt.HasValue)
        {
            throw new DomainException("Group is already disbanded.");
        }

        DisbandedAt = DateTime.UtcNow;
    }

    public bool IsAdmin(long userId) => Members.Any(x => x.MemberId == userId && x.Role == ConversationMemberRole.Admin);
    public bool IsMember(long userId) => Members.Any(x => x.MemberId == userId);

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

        var member = ConversationMember.Create(memberId, role);
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