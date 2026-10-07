using RealtimeChat.Application.Features.Media;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Stickers;

public static class StickerMapper
{
    public static StickerResponse ToResponse(Sticker sticker)
    {
        return new StickerResponse
        {
            PublicId = sticker.PublicId,
            Label = sticker.Label,
            Url = MediaMapper.MediaUrl(sticker.Media)
        };
    }
}