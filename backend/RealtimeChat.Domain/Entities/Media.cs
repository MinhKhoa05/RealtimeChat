using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Media
{
    public long Id { get; private set; }
    public Guid PublicId { get; private set; }
    public string OriginalName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Media() { }

    public static Media Create(string originalName, string contentType, long size, string extension)
    {
        if (string.IsNullOrWhiteSpace(originalName))
            throw new DomainException("Original name is required.");

        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException("Content type is required.");

        if (size <= 0)
            throw new DomainException("File size must be positive.");

        var publicId = Guid.NewGuid();

        return new Media
        {
            PublicId = publicId,
            OriginalName = originalName,
            ContentType = contentType,
            Size = size,
            StorageKey = $"{publicId:N}{extension}",
        };
    }
}