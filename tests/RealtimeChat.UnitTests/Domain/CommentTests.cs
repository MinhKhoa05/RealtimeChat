using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class CommentTests
{
    private const long DefaultAuthorId = 1;
    private const long DefaultNonAuthorId = 2;
    private const string DefautlContent = "Test Comment";
    private static Comment CreateDefault() => Comment.Create(1, DefaultAuthorId, DefautlContent);

    [Fact]
    public void Create_WithValidInput_CreatesComment()
    {
        var comment = Comment.Create(1, DefaultAuthorId, DefautlContent);

        comment.PostId.Should().Be(1);
        comment.AuthorId.Should().Be(DefaultAuthorId);
        comment.Content.Should().Be(DefautlContent);    
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Create_WhenContentIsNullOrEmpty_ThrowsDomainException(string? content)
    {
        var act = () => Comment.Create(1, DefaultAuthorId, content!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Delete_WithValidInput_SetDeletedAt()
    {
        var comment = CreateDefault();
        var actorId = DefaultAuthorId;
        var now = DateTime.UtcNow;

        var isChange = comment.Delete(actorId, now);

        comment.DeletedAt.Should().Be(now);
        isChange.Should().BeTrue();
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