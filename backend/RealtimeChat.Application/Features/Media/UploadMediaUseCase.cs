using RealtimeChat.Application.Interfaces;
using MediaEntity = RealtimeChat.Domain.Entities.Media;

namespace RealtimeChat.Application.Features.Media;

public class UploadMediaUseCase
{
    private readonly IAppDbContext _context;
    private readonly IFileStorage _storage;

    public UploadMediaUseCase(IAppDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<MediaResponse> ExecuteAsync(UploadMediaRequest request, CancellationToken ct)
    {
        var extension = GetExtension(request.ContentType);

        var media = MediaEntity.Create(request.OriginalName, request.ContentType, request.Size, extension);

        await _storage.SaveAsync(request.Content, media.StorageKey, ct);

        _context.Medias.Add(media);
        await _context.SaveChangesAsync(ct);

        return MediaMapper.ToResponse(media);
    }

    private static string GetExtension(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => throw new InvalidOperationException(
                $"Unsupported content type: {contentType}")
        };
    }
}

public class UploadMediaRequest
{
    public Stream Content { get; init; } = null!;
    public string OriginalName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }
}