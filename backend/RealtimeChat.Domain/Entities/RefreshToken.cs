namespace RealtimeChat.Domain.Entities;

public class RefreshToken
{
    public long Id { get; set; }
    
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = null!;
    public DateTime ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}