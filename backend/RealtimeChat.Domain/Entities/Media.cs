namespace RealtimeChat.Domain.Entities;

public class Media
{
    public long Id { get; private set; }
    public string OriginalName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Media() { }

    public static Media Create(string originalName, string contentType, long size, string storageKey)
    {
        return new Media
        {
            OriginalName = originalName,
            ContentType = contentType,
            Size = size,
            StorageKey = storageKey,
        };
    }
}