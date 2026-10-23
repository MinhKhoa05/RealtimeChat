using FluentAssertions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.UnitTests.Domain;

public class MediaTests
{
    [Fact]
    public void Create_WithValidInput_CreateMedia()
    {
        var originalName = "avatar.png";
        var contentType = "image/png";
        var size = 1024L;
        var extension = ".png";

        var media = Media.Create(originalName, contentType, size, extension);

        media.PublicId.Should().NotBeEmpty();
        media.OriginalName.Should().Be(originalName);
        media.ContentType.Should().Be(contentType);
        media.Size.Should().Be(size);
        media.StorageKey.Should().Be($"{media.PublicId:N}{extension}");
    }

    [Fact]
    public void Create_Twice_GenerateDifferentPublicIds()
    {
        var first = Media.Create("first.png", "image/png", 1024, ".png");

        var second = Media.Create("second.png", "image/png", 2048, ".png");

        first.PublicId.Should().NotBe(second.PublicId);
        first.StorageKey.Should().NotBe(second.StorageKey);
    }
}
