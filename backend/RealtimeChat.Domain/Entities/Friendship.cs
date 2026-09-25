namespace RealtimeChat.Domain.Entities;

public class Friendship
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long FriendId { get; set; }
    public User Friend { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}