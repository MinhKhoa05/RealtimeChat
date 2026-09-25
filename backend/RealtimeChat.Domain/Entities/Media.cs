namespace RealtimeChat.Domain.Entities;

public class Media
{
    public long Id { get; set; }
    public string OriginalName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string StorageKey { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}