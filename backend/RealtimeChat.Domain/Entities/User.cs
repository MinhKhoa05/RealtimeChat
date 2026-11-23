using RealtimeChat.Domain.Exceptions;

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
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        return new User
        {
            Name = name.Trim(),
            Email = email.Trim(),
            PasswordHash = passwordHash,
        };
    }

    public bool MaskLastSeen(DateTime now)
    {
        if (LastSeenAt.HasValue && now <= LastSeenAt.Value)
            return false;
            
        LastSeenAt = now;
        return true;
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        PasswordHash = passwordHash;
    }
}