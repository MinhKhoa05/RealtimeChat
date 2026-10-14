using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Comment : BaseEntity
{
    public long PostId { get; private set; }
    public Post Post { get; private set; } = null!;

    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string Content { get; private set; } = null!;

    public DateTime? DeletedAt { get; private set; }

    private Comment() { }

    public static Comment Create(long postId, long userId, string content)
    {
        return new Comment
        {
            PostId = postId,
            UserId = userId,
            Content = content,
        };
    }

    public void Delete(long actorId, DateTime now)
    {
        if (actorId != UserId)
        {
            throw new ForbiddenDomainException();
        }
        
        if (DeletedAt.HasValue)
        {
            throw new DomainException("Comment has been deleted");
        }

        DeletedAt = now;
    }
}