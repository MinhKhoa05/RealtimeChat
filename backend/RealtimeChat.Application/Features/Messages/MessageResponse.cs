using RealtimeChat.Domain.Enums;
using RealtimeChat.Application.Features.Call;
using RealtimeChat.Application.Features.Media;
using RealtimeChat.Application.Features.Stickers;

public class MessageResponse
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long? SenderId { get; set; }
    public MessageType Type { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public MediaResponse? Media { get; set; }
    public StickerResponse? Sticker { get; set; }
    public CallResponse? Call { get; set; }
}