using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Comment : BaseEntity
{
    public long PostId { get; private set; }
    public Post Post { get; private set; } = null!;

    public long AuthorId { get; private set; }
    public User Author { get; private set; } = null!;

    public string Content { get; private set; } = null!;

    public DateTime? DeletedAt { get; private set; }

    private Comment() { }

    public static Comment Create(long postId, long authorId, string content)
    {
        return new Comment
        {
            PostId = postId,
            AuthorId = authorId,
            Content = content,
        };
    }

    public void Delete(long actorId, DateTime now)
    {
        if (actorId != AuthorId)
        {
            throw new ForbiddenDomainException();
        }
        
        if (DeletedAt.HasValue)
        {
            throw new DomainException("Comment is already deleted");
        }

        DeletedAt = now;
    }
}