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

    public string? DirectKey { get; private set; } // Ngăn tạo trùng Direct chat khi có request đồng thời.
    public Guid? InviteCode { get; private set; } // Cho phép người dùng tham gia nhóm qua link mời.

    public ICollection<ConversationMember> Members { get; private set; } = new List<ConversationMember>();

    private Conversation() { }

    public static Conversation CreateDirect(long userId, long otherUserId, string directKey)
    {
        if (userId == otherUserId)
        {
            throw new DomainException("Cannot create private conversation with yourself");
        }

        var conversation = new Conversation
        {
            Type = ConversationType.Direct,
            DirectKey = directKey
        };

        var member1 = ConversationMember.CreateMember(userId);
        var member2 = ConversationMember.CreateMember(otherUserId);

        conversation.Members.Add(member1);
        conversation.Members.Add(member2);

        return conversation;
    }

    public static Conversation CreateGroup(long actorId, string name, IList<long> memberIds)
    {
        if (memberIds.Count != memberIds.Distinct().Count())
        {
            throw new DomainException("Duplicate member IDs.");
        }

        if (memberIds.Contains(actorId))
        {
            throw new DomainException("Creator must not be included in member IDs.");
        }

        if (memberIds.Count() < 2)
        {
            throw new DomainException("A group must have at least two invited members.");
        }

        var conversation = new Conversation
        {
            Name = name,
            Type = ConversationType.Group,
            InviteCode = Guid.NewGuid(),
        };

        var admin = ConversationMember.CreateAdmin(actorId);
        conversation.Members.Add(admin);

        foreach (var memberId in memberIds)
        {
            var member = ConversationMember.CreateMember(memberId, actorId);
            conversation.Members.Add(member);
        }

        return conversation;
    }

    public void AddMember(long actorId, long memberId)
    {
        EnsureGroupActive();

        if (actorId == memberId)
        {
            throw new DomainException("You cannot add yourself to the group.");
        }

        // Chỉ thành viên của nhóm mới được thêm thành viên
        var actor = FindMember(actorId)
            ?? throw new ForbiddenDomainException();

        var member = FindMember(memberId);
        if (member is not null)
        {
            throw new DomainException("User is already member.");
        }

        member = ConversationMember.CreateMember(memberId, actorId);
        Members.Add(member);
    }

    public void JoinByInvite(long actorId)
    {
        var member = FindMember(actorId);

        if (member is not null)
        {
            throw new DomainException("User is already a member.");
        }

        member = ConversationMember.CreateMember(actorId);
        Members.Add(member);
    }

    public void KickMember(long actorId, long memberId)
    {
        EnsureGroupActive();

        if (actorId == memberId)
        {
            throw new DomainException();
        }

        var actor = GetMemberOrThrow(actorId);
        if (!actor.IsAdmin())
        {
            throw new ForbiddenDomainException();
        }

        var member = GetMemberOrThrow(memberId);
        Members.Remove(member);
    }

    public void Leave(long actorId)
    {
        EnsureGroupActive();

        var member = GetMemberOrThrow(actorId);
        if (member.IsAdmin())
        {
            throw new DomainException("Admin cannot leave.");
        }

        Members.Remove(member);
    }

    public void TransferAdmin(long actorId, long newAdminId)
    {
        EnsureGroupActive();

        if (actorId == newAdminId)
        {
            throw new DomainException("Cannot transfer admin role to yourself.");
        }

        var actor = GetMemberOrThrow(actorId);
        if (!actor.IsAdmin())
        {
            throw new ForbiddenDomainException();
        }

        var target = GetMemberOrThrow(newAdminId);

        actor.SetMember();
        target.SetAdmin();
    }

    public void DisbandGroup(long actorId, DateTime now)
    {
        EnsureGroupActive();

        var actor = GetMemberOrThrow(actorId);
        if (!actor.IsAdmin())
        {
            throw new ForbiddenDomainException();
        }

        DisbandedAt = now;
    }

    private ConversationMember? FindMember(long memberId)
    {
        return Members.FirstOrDefault(x => x.MemberId == memberId);
    }

    private ConversationMember GetMemberOrThrow(long memberId)
    {
        return FindMember(memberId)
            ?? throw new DomainException("User is not a member.");
    }

    private void EnsureGroupActive()
    {
        if (Type != ConversationType.Group)
        {
            throw new DomainException("This operation is only available for group conversations.");
        }

        if (DisbandedAt.HasValue)
        {
            throw new DomainException("Group has been disbanded.");
        }
    }
}