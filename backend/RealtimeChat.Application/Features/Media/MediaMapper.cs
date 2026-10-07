using MediaEntity = RealtimeChat.Domain.Entities.Media;

namespace RealtimeChat.Application.Features.Media;

public static class MediaMapper
{
    public static string MediaUrl(MediaEntity media) => $"/media/{media.StorageKey}";

    public static MediaResponse ToResponse(MediaEntity media)
    {
        return new MediaResponse
        {
            PublicId = media.PublicId,
            OriginalName = media.OriginalName,
            ContentType = media.ContentType,
            Size = media.Size,
            Url = MediaUrl(media)
        };
    }
}