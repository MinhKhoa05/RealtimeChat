using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class MediaTests
{
    private const string DefaultOriginalName = "avatar.png";
    private const string DefaultContentType = "image/png";
    private const long DefaultSize = 1024L;
    private const string DefaultExtension = ".png";

    [Fact]
    public void Create_WithValidInput_CreatesMedia()
    {
        var media = Media.Create(DefaultOriginalName, DefaultContentType, DefaultSize, DefaultExtension);

        media.PublicId.Should().NotBeEmpty();
        media.OriginalName.Should().Be(DefaultOriginalName);
        media.ContentType.Should().Be(DefaultContentType);
        media.Size.Should().Be(DefaultSize);
        media.StorageKey.Should().Be($"{media.PublicId:N}{DefaultExtension}");
    }

    [Fact]
    public void Create_WhenCalledMultipleTimes_GeneratesDistinctPublicIdsAndStorageKeys()
    {
        var first = Media.Create("first.png", DefaultContentType, 1024L, DefaultExtension);
        var second = Media.Create("second.png", DefaultContentType, 2048L, DefaultExtension);

        first.PublicId.Should().NotBe(second.PublicId);
        first.StorageKey.Should().NotBe(second.StorageKey);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenOriginalNameIsNullOrWhiteSpace_ThrowsDomainException(string? originalName)
    {
        var act = () => Media.Create(originalName!, DefaultContentType, DefaultSize, DefaultExtension);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenContentTypeIsNullOrWhiteSpace_ThrowsDomainException(string? contentType)
    {
        var act = () => Media.Create(DefaultOriginalName, contentType!, DefaultSize, DefaultExtension);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    [InlineData(-1024L)]
    public void Create_WhenSizeIsZeroOrNegative_ThrowsDomainException(long size)
    {
        var act = () => Media.Create(DefaultOriginalName, DefaultContentType, size, DefaultExtension);

        act.Should().Throw<DomainException>();
    }
}
