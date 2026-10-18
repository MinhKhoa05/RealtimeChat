using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class CommentTests
{
    private const long DefaultAuthorId = 1;
    private const long DefaultNonAuthorId = 2;
    private static Comment CreateDefault() => Comment.Create(1, DefaultAuthorId, "Hi");

    [Fact]
    public void Delete_WithValidInput_SetDeletedAt()
    {
        var comment = CreateDefault();
        var actorId = DefaultAuthorId;
        var now = DateTime.UtcNow;

        comment.Delete(actorId, now);

        comment.DeletedAt.Should().Be(now);
    }

    [Fact]
    public void Delete_WhenActorIsNotAuthor_ThrowsForbiddenDomainException()
    {
        var comment = CreateDefault();
        var actorId = DefaultNonAuthorId;

        var act = () => comment.Delete(actorId, DateTime.UtcNow);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ReturnFalse()
    {
        var comment = CreateDefault();
        comment.Delete(DefaultAuthorId, DateTime.UtcNow);

        var result = comment.Delete(DefaultAuthorId, DateTime.UtcNow);

        result.Should().BeFalse();
    }
}