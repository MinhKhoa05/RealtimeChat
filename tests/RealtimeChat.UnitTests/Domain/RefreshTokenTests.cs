using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class RefreshTokenTests
{
    private const long DefaultUserId = 1L;
    private const string DefaultTokenHash = "hashed_refresh_token_value";

    private static readonly DateTime DefaultNow =
        new(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);

    private static RefreshToken CreateDefault()
        => RefreshToken.Create(DefaultUserId, DefaultTokenHash, DefaultNow);

    [Fact]
    public void Create_WithValidInput_CreatesActiveRefreshTokenWithSevenDaysLifetime()
    {
        var token = CreateDefault();

        token.UserId.Should().Be(DefaultUserId);
        token.TokenHash.Should().Be(DefaultTokenHash);
        token.ExpiresAt.Should().Be(DefaultNow.AddDays(7));
        token.RevokedAt.Should().BeNull();
        token.IsRevoked().Should().BeFalse();
        token.IsExpired(DefaultNow).Should().BeFalse();
        token.IsUsable(DefaultNow).Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenTokenHashIsNullOrWhiteSpace_ThrowsDomainException(string? tokenHash)
    {
        var act = () => RefreshToken.Create(DefaultUserId, tokenHash!, DefaultNow);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Revoke_WhenTokenIsActive_SetsRevokedAtAndMarksUnusable()
    {
        var token = CreateDefault();
        var revokeTime = DefaultNow.AddHours(1);

        token.Revoke(revokeTime);

        token.RevokedAt.Should().Be(revokeTime);
        token.IsRevoked().Should().BeTrue();
        token.IsUsable(revokeTime).Should().BeFalse();
    }

    [Fact]
    public void Revoke_WhenTokenIsAlreadyRevoked_ThrowsDomainException()
    {
        var token = CreateDefault();
        token.Revoke(DefaultNow.AddHours(1));

        var act = () => token.Revoke(DefaultNow.AddHours(2));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void IsExpired_WhenCurrentTimeIsBeforeExpiresAt_ReturnsFalse()
    {
        var token = CreateDefault();
        var beforeExpiry = token.ExpiresAt.AddTicks(-1);

        token.IsExpired(beforeExpiry).Should().BeFalse();
        token.IsUsable(beforeExpiry).Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void IsExpired_WhenCurrentTimeIsAtOrAfterExpiresAt_ReturnsTrueAndMarksUnusable(int extraMinutes)
    {
        var token = CreateDefault();
        var atOrAfterExpiry = token.ExpiresAt.AddMinutes(extraMinutes);

        token.IsExpired(atOrAfterExpiry).Should().BeTrue();
        token.IsUsable(atOrAfterExpiry).Should().BeFalse();
    }
}
