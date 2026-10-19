using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Conversation : BaseEntity
{
    public ConversationType Type { get; private set; }
    
    // Dùng cho Direct Chat
    public string? DirectKey { get; private set; }

    // Dùng cho Group Chat
    public string? Name { get; private set; }
    public long? AvatarMediaId { get; private set; }
    public Media? AvatarMedia { get; private set; }
    public DateTime? DisbandedAt { get; private set; }
    public Guid? InviteCode { get; private set; }

    private readonly List<ConversationMember> _members = [];
    public IReadOnlyCollection<ConversationMember> Members => _members.AsReadOnly();

    private Conversation() { }

    public static Conversation CreateDirect(long userId, long otherUserId)
    {
        if (userId == otherUserId)
            throw new DomainException("Cannot create private conversation with yourself.");

        var conversation = new Conversation
        {
            Type = ConversationType.Direct,
            DirectKey = CreateDirectKey(userId, otherUserId)
        };

        conversation._members.Add(ConversationMember.CreateMember(userId));
        conversation._members.Add(ConversationMember.CreateMember(otherUserId));

        return conversation;
    }

    public static Conversation CreateGroup(long actorId, string name, IList<long> inviteeUserIds)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Group name is required.");

        if (inviteeUserIds.Count != inviteeUserIds.Distinct().Count())
            throw new DomainException("Duplicate invitee IDs.");

        if (inviteeUserIds.Contains(actorId))
            throw new DomainException("Creator must not be included in invitee IDs.");

        if (inviteeUserIds.Count < 2)
            throw new DomainException("A group must have at least two invited members.");

        var conversation = new Conversation
        {
            Name = name,
            Type = ConversationType.Group,
            InviteCode = Guid.NewGuid()
        };

        conversation._members.Add(ConversationMember.CreateAdmin(actorId));

        foreach (var memberId in inviteeUserIds)
        {
            conversation._members.Add(ConversationMember.CreateMember(memberId, actorId));
        }

        return conversation;
    }

    public void EnsureCanSendMessage(long senderId)
    {
        if (DisbandedAt.HasValue)
            throw new DomainException("Conversation is inactive");

        if (FindMember(senderId) is null)
            throw new ForbiddenDomainException();
    }

    public bool UpdateReadState(long memberId, long messageId)
    {
        var member = GetMemberOrThrow(memberId);
        return member.MarkAsRead(messageId);
    }

    public void SetPinned(long memberId, bool isPinned)
    {
        var member = GetMemberOrThrow(memberId);
        member.SetPinned(isPinned);
    }

    public bool AddMember(long actorId, long memberId)
    {
        EnsureGroupActive();

        if (actorId == memberId)
        {
            throw new DomainException("You cannot add yourself to the group.");
        }

        if (FindMember(actorId) is null)
        {
            throw new ForbiddenDomainException();
        }

        if (FindMember(memberId) is not null)
        {
            return false;
        }

        _members.Add(ConversationMember.CreateMember(memberId, actorId));
        return true;
    }

    public bool JoinByInviteKey(long actorId)
    {
        EnsureGroupActive();

        if (FindMember(actorId) is not null)
        {
            return false;
        }

        _members.Add(ConversationMember.CreateMember(actorId));
        return true;
    }

    public bool KickMember(long actorId, long memberId)
    {
        EnsureGroupActive();

        if (actorId == memberId)
        {
            throw new DomainException("Cannot kick yourself. Use Leave instead.");
        }

        var actor = FindMember(actorId);

        if (actor is null || !actor.IsAdmin())
        {
            throw new ForbiddenDomainException();
        }

        var member = FindMember(memberId);

        if (member is null)
        {
            return false;
        }

        _members.Remove(member);
        return true;
    }

    public bool Leave(long actorId)
    {
        EnsureGroupActive();

        var member = GetMemberOrThrow(actorId);

        if (member.IsAdmin())
        {
            throw new DomainException("Admin cannot leave.");
        }

        _members.Remove(member);
        return true;
    }

    public bool TransferAdmin(long actorId, long newAdminId)
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

        var newAdmin = GetMemberOrThrow(newAdminId);

        actor.SetRole(ConversationMemberRole.Member);
        newAdmin.SetRole(ConversationMemberRole.Admin);
        return true;
    }

    public bool DisbandGroup(long actorId, DateTime now)
    {
        EnsureGroup();

        var actor = GetMemberOrThrow(actorId);

        if (!actor.IsAdmin())
        {
            throw new ForbiddenDomainException();
        }

        if (DisbandedAt.HasValue)
        {
            return false;
        }

        DisbandedAt = now;
        return true;
    }

    public static string CreateDirectKey(long userId, long otherUserId)
    {
        var min = Math.Min(userId, otherUserId);
        var max = Math.Max(userId, otherUserId);

        return $"{min}:{max}";
    }

    private ConversationMember? FindMember(long memberId)
    {
        return _members.FirstOrDefault(member => member.MemberId == memberId);
    }

    private ConversationMember GetMemberOrThrow(long memberId)
    {
        return FindMember(memberId)
            ?? throw new DomainException("User is not a member.");
    }

    private void EnsureGroup()
    {
        if (Type != ConversationType.Group)
        {
            throw new DomainException("This operation is only available for group conversations.");
        }
    }

    private void EnsureGroupActive()
    {
        EnsureGroup();

        if (DisbandedAt.HasValue)
        {
            throw new DomainException("Group has been disbanded.");
        }
    }
}
