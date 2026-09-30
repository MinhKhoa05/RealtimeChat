using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class Relationship
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public long TargetUserId { get; private set; }
    public User TargetUser { get; private set; } = null!;

    public RelationshipType Type { get; private set; }

    public string? Introduction { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Relationship() { }

    public static Relationship CreateFriend(long userId, long friendId)
    {
        return new Relationship
        {
            UserId = Math.Min(userId, friendId),
            TargetUserId = Math.Max(userId, friendId),
            Type = RelationshipType.Friend,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Relationship CreateBlock(long blockerId, long blockedId)
    {
        return new Relationship
        {
            UserId = blockerId,
            TargetUserId = blockedId,
            Type = RelationshipType.Block,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Relationship CreateFriendRequest(long senderId, long receiverId, string? introduction = null)
    {
        return new Relationship
        {
            UserId = senderId,
            TargetUserId = receiverId,
            Type = RelationshipType.FriendRequest,
            Introduction = introduction,
            CreatedAt = DateTime.UtcNow
        };
    }
}