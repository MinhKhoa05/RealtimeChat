using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class ConversationTests
{
    private const long DefaultGroupAdminId = 1L;
    private const string DefaultGroupName = "Test Group";
    private static readonly List<long> DefaultGroupMemberIds = [2, 3];
    private const long DefaultNewMemberId = 5L;

    private static readonly DateTime DefaultNow = new(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);

    private static Conversation CreateGroupDefault()
        => Conversation.CreateGroup(
            DefaultGroupAdminId,
            DefaultGroupName,
            new List<long>(DefaultGroupMemberIds));

    private static Conversation CreateConversationDefault(ConversationType type)
    {
        if (type == ConversationType.Group) return CreateGroupDefault();

        return Conversation.CreateDirect(1, 2);
    }

    [Fact]
    public void CreateDirect_WhenInputIsValid_CreatesDirectConversation()
    {
        var userId = 1L;
        var otherUserId = 2L;
        var directKey = Conversation.CreateDirectKey(userId, otherUserId);

        var conversation = Conversation.CreateDirect(userId, otherUserId);

        conversation.Type.Should().Be(ConversationType.Direct);
        conversation.DirectKey.Should().Be(directKey);
        conversation.Members.Should().HaveCount(2);
        conversation.Members.Should().ContainSingle(x => x.MemberId == userId);
        conversation.Members.Should().ContainSingle(x => x.MemberId == otherUserId);
    }

    [Fact]
    public void CreateDirect_WhenUserIdsAreEqual_ThrowsDomainException()
    {
        var userId = 1L;
        var otherUserId = 1L;

        var act = () => Conversation.CreateDirect(userId, otherUserId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateGroup_WhenInputIsValid_CreatesGroupConversation()
    {
        var actorId = DefaultGroupAdminId;
        var name = DefaultGroupName;
        var memberIds = new List<long>(DefaultGroupMemberIds);
        var memberCount = memberIds.Count + 1; // Members + group admin

        var conversation = Conversation.CreateGroup(actorId, name, memberIds);

        conversation.Type.Should().Be(ConversationType.Group);
        conversation.Name.Should().Be(name);
        conversation.InviteCode.Should().NotBeNull();
        conversation.InviteCode.Should().NotBe(Guid.Empty);

        conversation.Members.Should().HaveCount(memberCount);
        conversation.Members.Should().ContainSingle(x =>
            x.MemberId == actorId &&
            x.Role == ConversationMemberRole.Admin);

        conversation.Members
            .Select(x => x.MemberId)
            .Should()
            .BeEquivalentTo(memberIds.Append(actorId));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void CreateGroup_WhenNameIsNullOrWhiteSpace_ThrowsDomainException(string? name)
    {
        var act = () => Conversation.CreateGroup(
            DefaultGroupAdminId,
            name!,
            new List<long>(DefaultGroupMemberIds));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateGroup_WhenMemberIdsContainDuplicates_ThrowsDomainException()
    {
        var memberIds = new List<long> { 2, 2, 3, 4 };

        var act = () => Conversation.CreateGroup(DefaultGroupAdminId, DefaultGroupName, memberIds);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateGroup_WhenMemberIdsContainActorId_ThrowsDomainException()
    {
        var actorId = DefaultGroupAdminId;
        var memberIds = new List<long>(DefaultGroupMemberIds);
        memberIds.Add(actorId);

        var act = () => Conversation.CreateGroup(actorId, DefaultGroupName, memberIds);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void CreateGroup_WhenMemberCountIsLessThanTwo_ThrowsDomainException(int memberCount)
    {
        var memberIds = new List<long>();
        for (int i = 0; i < memberCount; i++)
        {
            memberIds.Add(DefaultGroupAdminId + i + 1); // Generate IDs distinct from the admin ID
        }

        var act = () => Conversation.CreateGroup(DefaultGroupAdminId, DefaultGroupName, memberIds);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void EnsureCanSendMessage_WhenConversationIsDisbanded_ThrowsDomainException()
    {
        // Chỉ có group mới có disbanded
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);
        var memberId = conversation.Members.First().MemberId;

        var act = () => conversation.EnsureCanSendMessage(memberId);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(ConversationType.Group)]
    [InlineData(ConversationType.Direct)]
    public void EnsureCanSendMessage_WhenSenderIsNotMember_ThrowsForbiddenDomainException(ConversationType type)
    {
        var conversation = CreateConversationDefault(type);
        var senderId = DefaultNewMemberId;

        var act = () => conversation.EnsureCanSendMessage(senderId);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Theory]
    [InlineData(ConversationType.Direct)]
    [InlineData(ConversationType.Group)]
    public void EnsureCanSendMessage_WhenSenderIsMember_DoesNotThrow(ConversationType type)
    {
        var conversation = CreateConversationDefault(type);
        var senderId = 1;

        var act = () => conversation.EnsureCanSendMessage(senderId);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(ConversationType.Direct)]
    [InlineData(ConversationType.Group)]
    public void UpdateReadState_WhenMemberExists_ReturnsTrue(ConversationType type)
    {
        var conversation = CreateConversationDefault(type);
        var member = conversation.Members.First();
        var messageId = 10;

        var result = conversation.UpdateReadState(member.MemberId, messageId);

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(ConversationType.Direct)]
    [InlineData(ConversationType.Group)]
    public void UpdateReadState_WhenMessageIdIsOlderThanLastReadMessageId_DoesNotMoveBackward(ConversationType type)
    {
        var conversation = CreateConversationDefault(type);
        var member = conversation.Members.First();
        conversation.UpdateReadState(member.MemberId, 10);

        var isChange = conversation.UpdateReadState(member.MemberId, 5);

        isChange.Should().BeFalse();
        member.LastReadMessageId.Should().Be(10);
    }

    [Theory]
    [InlineData(ConversationType.Direct)]
    [InlineData(ConversationType.Group)]
    public void UpdateReadState_WhenMemberDoesNotExist_ThrowsDomainException(ConversationType type)
    {
        var conversation = CreateConversationDefault(type);
        var userId = DefaultNewMemberId;

        var act = () => conversation.UpdateReadState(userId, 1);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(ConversationType.Direct)]
    [InlineData(ConversationType.Group)]
    public void SetPinned_WhenMemberExists_SetsPinnedState(ConversationType type)
    {
        var conversation = CreateConversationDefault(type);
        var member = conversation.Members.First();

        conversation.SetPinned(member.MemberId, true);

        member.IsPinned.Should().BeTrue();
    }

    [Theory]
    [InlineData(ConversationType.Direct)]
    [InlineData(ConversationType.Group)]
    public void SetPinned_WhenMemberDoesNotExist_ThrowsDomainException(ConversationType type)
    {
        var conversation = CreateConversationDefault(type);
        var userId = DefaultNewMemberId;

        var act = () => conversation.SetPinned(userId, true);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddMember_WhenInputIsValid_AddsMemberToGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = DefaultNewMemberId;
        var memberCount = DefaultGroupMemberIds.Count + 2; // Members + group admin + new member

        var isChange = conversation.AddMember(actorId, memberId);

        isChange.Should().BeTrue();
        conversation.Members.Should().HaveCount(memberCount);
        conversation.Members.Should().ContainSingle(x => x.MemberId == memberId);
    }

    [Fact]
    public void AddMember_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L);
        var actorId = 1L;

        var act = () => conversation.AddMember(actorId, DefaultNewMemberId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddMember_WhenConversationIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var actorId = DefaultGroupAdminId;
        var memberId = 10L;

        var act = () => conversation.AddMember(actorId, memberId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddMember_WhenActorIsMemberBeingAdded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = DefaultGroupAdminId;

        var act = () => conversation.AddMember(actorId, memberId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddMember_WhenActorIdIsNotMember_ThrowsForbiddenDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = 15L;
        var memberId = 10L;

        var act = () => conversation.AddMember(actorId, memberId);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void AddMember_WhenMemberAlreadyExists_ReturnsFalse()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = 2L;

        var result = conversation.AddMember(actorId, memberId);

        result.Should().BeFalse();
    }

    [Fact]
    public void JoinByInviteKey_WhenInputIsValid_AddsMemberToGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = 10L;
        var memberCount = DefaultGroupMemberIds.Count + 2; // Members + group admin + joining member

        var isChange = conversation.JoinByInviteKey(actorId);

        isChange.Should().BeTrue();
        conversation.Members.Should().HaveCount(memberCount);
        conversation.Members.Should().ContainSingle(x => x.MemberId == actorId);
    }

    [Fact]
    public void JoinByInviteKey_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L);
        var actorId = 10L;

        var act = () => conversation.JoinByInviteKey(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void JoinByInviteKey_WhenConversationIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);
        var actorId = 10L;

        var act = () => conversation.JoinByInviteKey(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void JoinByInviteKey_WhenActorIsAlreadyMember_ReturnsFalse()
    {
        var conversation = CreateGroupDefault();
        var actorId = 2L;

        var result = conversation.JoinByInviteKey(actorId);

        result.Should().BeFalse();
    }

    [Fact]
    public void KickMember_WhenInputIsValid_RemovesMemberFromGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = 2L;

        var isChange = conversation.KickMember(actorId, memberId);

        isChange.Should().BeTrue();
        conversation.Members.Should().HaveCount(2);
        conversation.Members.Should().NotContain(x => x.MemberId == memberId);
    }

    [Fact]
    public void KickMember_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L);

        var act = () => conversation.KickMember(1L, 2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void KickMember_WhenConversationIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var act = () => conversation.KickMember(DefaultGroupAdminId, 2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void KickMember_WhenActorIsTargetMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = DefaultGroupAdminId;

        var act = () => conversation.KickMember(actorId, memberId);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(2L)]
    [InlineData(10L)]
    public void KickMember_WhenActorIsNotAdmin_ThrowsForbiddenDomainException(long actorId)
    {
        var conversation = CreateGroupDefault();
        var memberId = 3L;

        var act = () => conversation.KickMember(actorId, memberId);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void KickMember_WhenTargetIsNotMember_ReturnsFalse()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = 10L;

        var result = conversation.KickMember(actorId, memberId);

        result.Should().BeFalse();
    }

    [Fact]
    public void Leave_WhenInputIsValid_RemovesMemberFromGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = 2L;

        var isChange =conversation.Leave(actorId);

        isChange.Should().BeTrue();
        conversation.Members.Should().HaveCount(2);
        conversation.Members.Should().NotContain(x => x.MemberId == actorId);
    }

    [Fact]
    public void Leave_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L);

        var act = () => conversation.Leave(1L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Leave_WhenConversationIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var act = () => conversation.Leave(2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Leave_WhenActorIsAdmin_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();

        var act = () => conversation.Leave(DefaultGroupAdminId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Leave_WhenActorIsNotMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = 10L;

        var act = () => conversation.Leave(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenInputIsValid_TransfersAdminRole()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var newAdminId = 2L;

        var isChnage = conversation.TransferAdmin(actorId, newAdminId);

        isChnage.Should().BeTrue();

        conversation.Members.Should().ContainSingle(x =>
            x.MemberId == newAdminId &&
            x.Role == ConversationMemberRole.Admin);

        conversation.Members.Should().ContainSingle(x =>
            x.MemberId == actorId &&
            x.Role == ConversationMemberRole.Member);
    }

    [Fact]
    public void TransferAdmin_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L);

        var act = () => conversation.TransferAdmin(1L, 2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenConversationIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var act = () => conversation.TransferAdmin(
            DefaultGroupAdminId,
            2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenActorIsTargetAdmin_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;

        var act = () => conversation.TransferAdmin(actorId, actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenActorIsNotAdmin_ThrowsForbiddenDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = 2L;
        var newAdminId = 3L;

        var act = () => conversation.TransferAdmin(actorId, newAdminId);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenTargetIsNotMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var newAdminId = 10L;

        var act = () => conversation.TransferAdmin(actorId, newAdminId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenActorIsNotMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = 10L;
        var newAdminId = 2L;

        var act = () => conversation.TransferAdmin(actorId, newAdminId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void DisbandGroup_WhenInputIsValid_SetsDisbandedAt()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;

        var isChange = conversation.DisbandGroup(actorId, DefaultNow);

        isChange.Should().BeTrue();
        conversation.DisbandedAt.Should().Be(DefaultNow);
    }

    [Fact]
    public void DisbandGroup_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L);

        var act = () => conversation.DisbandGroup(1L, DefaultNow);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void DisbandGroup_WhenGroupIsAlreadyDisbanded_ReturnsFalse()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var result = conversation.DisbandGroup(
            DefaultGroupAdminId,
            DefaultNow.AddMinutes(1));

        result.Should().BeFalse();
    }

    [Fact]
    public void DisbandGroup_WhenActorIsNotAdmin_ThrowsForbiddenDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupMemberIds[0];

        var act = () => conversation.DisbandGroup(actorId, DefaultNow);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void DisbandGroup_WhenActorIsNotMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultNewMemberId;

        var act = () => conversation.DisbandGroup(actorId, DefaultNow);

        act.Should().Throw<DomainException>();
    }
}
