using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using FluentAssertions;

namespace RealtimeChat.UnitTests.Domain;

public class ConversationMemberTests
{
    private const long DefaultMemberId = 1;
    private static ConversationMember CreateMemberDefault() => ConversationMember.CreateMember(DefaultMemberId);

    // Test 2 trường hợp, có người mời hoặc không có người mời (link tham gia nhóm)
    [Theory]
    [InlineData(null)]
    [InlineData(2L)]
    public void CreateMember_WithValidInput_CreateMember(long? invitedById)
    {
        var member = ConversationMember.CreateMember(DefaultMemberId, invitedById);

        member.MemberId.Should().Be(DefaultMemberId);
        member.Role.Should().Be(ConversationMemberRole.Member);
        member.InvitedById.Should().Be(invitedById);
    }

    [Fact]
    public void CreateAdmin_WithValidInput_CreateAdmin()
    {
        var admin = ConversationMember.CreateAdmin(DefaultMemberId);

        admin.MemberId.Should().Be(DefaultMemberId);
        admin.Role.Should().Be(ConversationMemberRole.Admin);
    }

    [Fact]
    public void MarkAsRead_WithValidInput_SetsLastReadMessageId()
    {
        var member = CreateMemberDefault();

        member.MarkAsRead(10L);

        member.LastReadMessageId.Should().Be(10L);
    }

    [Fact]
    public void MarkAsRead_WhenMessageIdIsGreater_UpdatesLastReadMessageId()
    {
        var member = CreateMemberDefault();
        member.MarkAsRead(10L);

        // Act
        member.MarkAsRead(20L);

        member.LastReadMessageId.Should().Be(20L);
    }

    [Fact]
    public void MarkAsRead_WhenMessageIdIsSmaller_KeepsLastReadMessageId()
    {
        var member = CreateMemberDefault();
        member.MarkAsRead(20L);

        // Act
        member.MarkAsRead(10L);

        member.LastReadMessageId.Should().Be(20L);
    }
}