using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class RefreshToken
{
    public long Id { get; private set; }

    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string TokenHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private const int LifetimeDays = 7;

    private RefreshToken() { }

    public static RefreshToken Create(long userId, string tokenHash)
    {
        var now = DateTime.UtcNow;

        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(LifetimeDays),
        };
    }

    public void Revoke()
    {
        if (IsRevoked())
        {
            throw new DomainException("Token has already been revoked.");
        }

        RevokedAt = DateTime.UtcNow;
    }

    public bool IsRevoked() => RevokedAt.HasValue;

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;

    public bool IsUsable() => !IsRevoked() && !IsExpired();
}