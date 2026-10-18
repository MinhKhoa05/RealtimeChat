using RealtimeChat.Domain.Entities;
using FluentAssertions;

namespace RealtimeChat.UnitTests.Domain;

public class ConversationMemberTests
{
    private static ConversationMember CreateMemberDefault()
    {
        var conversation = Conversation.CreateGroup(
            actorId: 1,
            name: "Test Group",
            memberIds: new List<long> { 2, 3 });

        return conversation.Members
            .Single(x => x.MemberId == 2);
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