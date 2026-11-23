using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class UserTests
{
    private const string DefaultName = "John Doe";
    private const string DefaultEmail = "john.doe@example.com";
    private const string DefaultPasswordHash = "hashed_password_123";

    private static readonly DateTime DefaultNow =
        new(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);

    private static User CreateDefault()
        => User.Create(DefaultName, DefaultEmail, DefaultPasswordHash);

    [Fact]
    public void Create_WithValidInput_CreatesUserAndTrimsNameAndEmail()
    {
        var user = User.Create("  John Doe  ", "  john.doe@example.com  ", DefaultPasswordHash);

        user.Name.Should().Be(DefaultName);
        user.Email.Should().Be(DefaultEmail);
        user.PasswordHash.Should().Be(DefaultPasswordHash);
        user.AvatarMediaId.Should().BeNull();
        user.LastSeenAt.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenNameIsNullOrWhiteSpace_ThrowsDomainException(string? name)
    {
        var act = () => User.Create(name!, DefaultEmail, DefaultPasswordHash);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenEmailIsNullOrWhiteSpace_ThrowsDomainException(string? email)
    {
        var act = () => User.Create(DefaultName, email!, DefaultPasswordHash);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenPasswordHashIsNullOrWhiteSpace_ThrowsDomainException(string? passwordHash)
    {
        var act = () => User.Create(DefaultName, DefaultEmail, passwordHash!);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MaskLastSeen_WhenLastSeenAtIsNull_SetsLastSeenAtAndReturnsTrue()
    {
        var user = CreateDefault();

        var isChanged = user.MaskLastSeen(DefaultNow);

        isChanged.Should().BeTrue();
        user.LastSeenAt.Should().Be(DefaultNow);
    }

    [Fact]
    public void MaskLastSeen_WhenTimestampIsNewer_UpdatesLastSeenAtAndReturnsTrue()
    {
        var user = CreateDefault();
        user.MaskLastSeen(DefaultNow);
        var newerTime = DefaultNow.AddMinutes(5);

        var isChanged = user.MaskLastSeen(newerTime);

        isChanged.Should().BeTrue();
        user.LastSeenAt.Should().Be(newerTime);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void MaskLastSeen_WhenTimestampIsEqualOrOlder_ReturnsFalseAndDoesNotMoveBackward(int offsetMinutes)
    {
        var user = CreateDefault();
        user.MaskLastSeen(DefaultNow);
        var targetTime = DefaultNow.AddMinutes(offsetMinutes);

        var isChanged = user.MaskLastSeen(targetTime);

        isChanged.Should().BeFalse();
        user.LastSeenAt.Should().Be(DefaultNow);
    }

    [Fact]
    public void ChangePassword_WithValidHash_UpdatesPasswordHash()
    {
        var user = CreateDefault();
        var newPasswordHash = "new_hashed_password_456";

        user.ChangePassword(newPasswordHash);

        user.PasswordHash.Should().Be(newPasswordHash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangePassword_WhenPasswordHashIsNullOrWhiteSpace_ThrowsDomainException(string? newPasswordHash)
    {
        var user = CreateDefault();

        var act = () => user.ChangePassword(newPasswordHash!);

        act.Should().Throw<DomainException>();
    }
}
