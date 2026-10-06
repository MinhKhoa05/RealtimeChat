namespace RealtimeChat.Domain.Entities;

public class Media : BaseEntity
{
    public Guid PublicId { get; private set; }
    public string OriginalName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public string StorageKey { get; private set; } = null!;

    private Media() { }

    public static Media Create(string originalName, string contentType, long size, string storageKey)
    {
        return new Media
        {
            PublicId = Guid.NewGuid(),
            OriginalName = originalName,
            ContentType = contentType,
            Size = size,
            StorageKey = storageKey,
        };
    }
}