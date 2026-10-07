namespace RealtimeChat.Domain.Entities;

public class UserStickerCollection
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public long CollectionId { get; private set; }
    public StickerCollection Collection { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    private UserStickerCollection() { }

    public static UserStickerCollection Create(long userId, long collectionId)
    {
        return new UserStickerCollection
        {
            UserId = userId,
            CollectionId = collectionId,
            CreatedAt = DateTime.UtcNow,
        };
    }
}