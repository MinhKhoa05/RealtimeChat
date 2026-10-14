namespace RealtimeChat.Domain.Entities;

public class PostLike : BaseEntity
{
    public long PostId { get; private set; }
    public Post Post { get; private set; } = null!;

    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    private PostLike() { }

    public static PostLike Create(long postId, long userId)
    {
        return new PostLike
        {
            PostId = postId,
            UserId = userId,
        };
    }
}