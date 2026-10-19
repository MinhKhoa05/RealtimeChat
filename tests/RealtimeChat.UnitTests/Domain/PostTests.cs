using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class PostTests
{
    private const long DefaultAuthorId = 1;
    private const long DefaultNonAuthorId = 2;
    private static Post CreateDefault() => Post.Create(DefaultAuthorId, "Hi", PostPrivacy.Public);

    [Theory]
    [InlineData(PostPrivacy.Public)]
    [InlineData(PostPrivacy.Friends)]
    [InlineData(PostPrivacy.Private)]
    public void SetPrivacy_WithValidInput_UpdatePrivacy(PostPrivacy newPrivacy)
    {
        var post = CreateDefault();
        var actorId = DefaultAuthorId;

        post.SetPrivacy(actorId, newPrivacy);

        post.Privacy.Should().Be(newPrivacy);
    }

    [Fact]
    public void SetPrivacy_WhenActorIsNotAuthor_ThrowsForbiddenDomainException()
    {
        var post = CreateDefault();
        var actorId = DefaultNonAuthorId;

        var act = () => post.SetPrivacy(actorId, PostPrivacy.Private);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void Delete_WithValidInput_SetDeletedAt()
    {
        var post = CreateDefault();
        var actorId = DefaultAuthorId;
        var now = DateTime.UtcNow;

        var isChange = post.Delete(actorId, now);

        isChange.Should().BeTrue();
        post.DeletedAt.Should().Be(now);
    }

    [Fact]
    public void Delete_WhenActorIsNotAuthor_ThrowsForbiddenDomainException()
    {
        var post = CreateDefault();
        var actorId = DefaultNonAuthorId;

        var act = () => post.Delete(actorId, DateTime.UtcNow);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ReturnFalse()
    {
        var post = CreateDefault();
        post.Delete(DefaultAuthorId, DateTime.UtcNow);

        var result = post.Delete(DefaultAuthorId, DateTime.UtcNow);

        result.Should().BeFalse();
    }
}