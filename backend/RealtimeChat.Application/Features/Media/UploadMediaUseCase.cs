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

    public async Task<Guid> ExecuteAsync(UploadMediaRequest request, CancellationToken ct)
    {
        var storageKey = await _storage.SaveAsync(request.Content, request.ContentType, ct);

        var media = MediaEntity.Create(request.OriginalName, request.ContentType, request.Size, storageKey);

        _context.Medias.Add(media);
        await _context.SaveChangesAsync(ct);

        return media.PublicId;
    }
}

public class UploadMediaRequest
{
    public Stream Content { get; init; } = null!;
    public string OriginalName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long Size { get; init; }
}