namespace RealtimeChat.Domain.Entities;

public class UserBlock
{
    public long BlockerId { get; set; }
    public User Blocker { get; set; } = null!;

    public long BlockedUserId { get; set; }
    public User BlockedUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}