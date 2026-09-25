namespace RealtimeChat.Domain.Entities;

public class FriendRequest
{
    public long SenderId { get; set; }
    public User Sender { get; set; } = null!;

    public long ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;

    public string? Introduction { get; set; }

    public DateTime CreatedAt { get; set; }
}