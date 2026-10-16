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

    [Fact]
    public void CreateDirect_WithValidInput_CreatesDirectConversation()
    {
        var userId = 1L;
        var otherUserId = 2L;
        var directKey = "1:2";

        var conversation = Conversation.CreateDirect(userId, otherUserId, directKey);

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
        var directKey = "1:1";

        var act = () => Conversation.CreateDirect(userId, otherUserId, directKey);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateGroup_WithValidInput_CreatesGroup()
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
    public void CreateGroup_WhenMembersContainActor_ThrowsDomainException()
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
    public void CreateGroup_WhenMemberIdsCountIsLessThanTwo_ThrowsDomainException(int memberCount)
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
    public void AddMember_WithValidInput_AddsMemberToGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = DefaultNewMemberId;
        var memberCount = DefaultGroupMemberIds.Count + 2; // Members + group admin + new member

        conversation.AddMember(actorId, memberId);

        conversation.Members.Should().HaveCount(memberCount);
        conversation.Members.Should().ContainSingle(x => x.MemberId == memberId);
    }

    [Fact]
    public void AddMember_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L, "1:2");
        var actorId = 1L;

        var act = () => conversation.AddMember(actorId, DefaultNewMemberId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddMember_WhenGroupIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var actorId = DefaultGroupAdminId;
        var memberId = 10L;

        var act = () => conversation.AddMember(actorId, memberId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void AddMember_WhenActorIdEqualsMemberId_ThrowsDomainException()
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
    public void AddMember_WhenMemberIdIsAlreadyMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = 2L;

        var act = () => conversation.AddMember(actorId, memberId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void JoinByInvite_WithValidInput_AddsMemberToGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = 10L;
        var memberCount = DefaultGroupMemberIds.Count + 2; // Members + group admin + joining member

        conversation.JoinByInvite(actorId);

        conversation.Members.Should().HaveCount(memberCount);
        conversation.Members.Should().ContainSingle(x => x.MemberId == actorId);
    }

    [Fact]
    public void JoinByInvite_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L, "1:2");
        var actorId = 10L;

        var act = () => conversation.JoinByInvite(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void JoinByInvite_WhenGroupIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);
        var actorId = 10L;

        var act = () => conversation.JoinByInvite(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void JoinByInvite_WhenActorIdIsAlreadyMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = 2L;

        var act = () => conversation.JoinByInvite(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void KickMember_WithValidInput_RemovesMemberFromGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = 2L;

        conversation.KickMember(actorId, memberId);

        conversation.Members.Should().HaveCount(2);
        conversation.Members.Should().NotContain(x => x.MemberId == memberId);
    }

    [Fact]
    public void KickMember_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L, "1:2");

        var act = () => conversation.KickMember(1L, 2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void KickMember_WhenGroupIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var act = () => conversation.KickMember(DefaultGroupAdminId, 2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void KickMember_WhenActorIdEqualsMemberId_ThrowsDomainException()
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
    public void KickMember_WhenTargetIsNotMember_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var memberId = 10L;

        var act = () => conversation.KickMember(actorId, memberId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Leave_WithValidInput_RemovesMemberFromGroup()
    {
        var conversation = CreateGroupDefault();
        var actorId = 2L;

        conversation.Leave(actorId);

        conversation.Members.Should().HaveCount(2);
        conversation.Members.Should().NotContain(x => x.MemberId == actorId);
    }

    [Fact]
    public void Leave_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L, "1:2");

        var act = () => conversation.Leave(1L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Leave_WhenGroupIsDisbanded_ThrowsDomainException()
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
    public void TransferAdmin_WithValidInput_TransfersAdminRole()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;
        var newAdminId = 2L;

        conversation.TransferAdmin(actorId, newAdminId);

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
        var conversation = Conversation.CreateDirect(1L, 2L, "1:2");

        var act = () => conversation.TransferAdmin(1L, 2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenGroupIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var act = () => conversation.TransferAdmin(
            DefaultGroupAdminId,
            2L);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void TransferAdmin_WhenActorIdEqualsNewAdminId_ThrowsDomainException()
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
    public void DisbandGroup_WithValidInput_SetsDisbandedAt()
    {
        var conversation = CreateGroupDefault();
        var actorId = DefaultGroupAdminId;

        conversation.DisbandGroup(actorId, DefaultNow);

        conversation.DisbandedAt.Should().Be(DefaultNow);
    }

    [Fact]
    public void DisbandGroup_WhenConversationIsDirect_ThrowsDomainException()
    {
        var conversation = Conversation.CreateDirect(1L, 2L, "1:2");

        var act = () => conversation.DisbandGroup(1L, DefaultNow);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void DisbandGroup_WhenGroupIsDisbanded_ThrowsDomainException()
    {
        var conversation = CreateGroupDefault();
        conversation.DisbandGroup(DefaultGroupAdminId, DefaultNow);

        var act = () => conversation.DisbandGroup(
            DefaultGroupAdminId,
            DefaultNow.AddMinutes(1));

        act.Should().Throw<DomainException>();
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
