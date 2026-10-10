using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private static readonly TimeSpan Lifetime = TimeSpan.FromDays(7);

    private RefreshToken() { }

    public static RefreshToken Create(long userId, string tokenHash, DateTime now)
    {

        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = now.Add(Lifetime),
        };
    }

    public void Revoke(DateTime now)
    {
        if (IsRevoked())
        {
            throw new DomainException("Token has already been revoked.");
        }

        RevokedAt = now;
    }

    public bool IsRevoked() => RevokedAt.HasValue;

    public bool IsExpired(DateTime now) => now >= ExpiresAt;

    public bool IsUsable(DateTime now) => !IsRevoked() && !IsExpired(now);
}