namespace RealtimeChat.Application.Features.Stickers;

public class StickerResponse
{
    public Guid PublicId { get; set; }
    public string Label { get; set; } = null!;
    public string Url { get; set; } = null!;
}