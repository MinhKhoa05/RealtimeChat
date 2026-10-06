using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private const int LifetimeDays = 7;

    private RefreshToken() { }

    public static RefreshToken Create(long userId, string tokenHash)
    {

        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(LifetimeDays),
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