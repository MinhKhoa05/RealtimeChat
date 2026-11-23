using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class RelationshipTests
{
    private const long FirstUserId = 1L;
    private const long SecondUserId = 5L;

    [Theory]
    [InlineData(FirstUserId, SecondUserId)]
    [InlineData(SecondUserId, FirstUserId)]
    public void CreateFriend_WithValidUsers_CreatesFriendWithNormalizedUserIds(long userId, long friendId)
    {
        var relationship = Relationship.CreateFriend(userId, friendId);

        relationship.UserId.Should().Be(FirstUserId);
        relationship.TargetUserId.Should().Be(SecondUserId);
        relationship.Type.Should().Be(RelationshipType.Friend);
        relationship.Introduction.Should().BeNull();
    }

    [Fact]
    public void CreateFriend_WhenUserAndFriendAreEqual_ThrowsDomainException()
    {
        var act = () => Relationship.CreateFriend(FirstUserId, FirstUserId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateBlock_WithValidUsers_CreatesDirectionalBlockRelationship()
    {
        var relationship = Relationship.CreateBlock(SecondUserId, FirstUserId);

        relationship.UserId.Should().Be(SecondUserId);
        relationship.TargetUserId.Should().Be(FirstUserId);
        relationship.Type.Should().Be(RelationshipType.Block);
        relationship.Introduction.Should().BeNull();
    }

    [Fact]
    public void CreateBlock_WhenBlockerAndBlockedAreEqual_ThrowsDomainException()
    {
        var act = () => Relationship.CreateBlock(FirstUserId, FirstUserId);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Hi, let's connect!")]
    public void CreateFriendRequest_WithValidUsers_CreatesDirectionalFriendRequest(string? introduction)
    {
        var relationship = Relationship.CreateFriendRequest(SecondUserId, FirstUserId, introduction);

        relationship.UserId.Should().Be(SecondUserId);
        relationship.TargetUserId.Should().Be(FirstUserId);
        relationship.Type.Should().Be(RelationshipType.FriendRequest);
        relationship.Introduction.Should().Be(introduction);
    }

    [Fact]
    public void CreateFriendRequest_WhenSenderAndReceiverAreEqual_ThrowsDomainException()
    {
        var act = () => Relationship.CreateFriendRequest(FirstUserId, FirstUserId);

        act.Should().Throw<DomainException>();
    }
}
