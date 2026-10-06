namespace RealtimeChat.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public long? AvatarMediaId { get; private set; }
    public Media? AvatarMedia { get; private set; }

    public DateTime? LastSeenAt { get; private set; }

    private User() { }

    public static User Create(string name, string email, string passwordHash)
    {
        return new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
        };
    }

    public void MaskLastSeen()
    {
        LastSeenAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}