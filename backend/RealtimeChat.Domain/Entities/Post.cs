using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Post : BaseEntity
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public PostPrivacy Privacy { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public ICollection<Comment> Comments { get; private set; } = new List<Comment>();
    public ICollection<PostLike> Likes { get; private set; } = new List<PostLike>();

    private Post() { }

    public static Post Create(long userId, string content, PostPrivacy privacy)
    {
        return new Post
        {
            UserId = userId,
            Content = content,
            Privacy = privacy
        };
    }

    public void SetPrivacy(long actorId, PostPrivacy postPrivacy)
    {
        if (actorId != UserId)
        {
            throw new ForbiddenDomainException();
        }

        Privacy = postPrivacy;
    }

    public void Delete(long actorId, DateTime now)
    {
        if (actorId != UserId)
        {
            throw new ForbiddenDomainException();
        }

        if (DeletedAt.HasValue)
        {
            throw new DomainException("Post has been deleted");
        }

        DeletedAt = now;
    }
}