namespace RealtimeChat.Application.Features.Media;

public class MediaResponse
{
    public long Id { get; set; }
    public string OriginalName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public string Url { get; set; } = null!;
}