using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.Exceptions;
using MediaEntity = RealtimeChat.Domain.Entities.Media;

namespace RealtimeChat.Application.Features.Media;

public class GetMediaUseCase
{
    private readonly IAppDbContext _context;
    private readonly IFileStorage _storage;

    public GetMediaUseCase(IAppDbContext context, IFileStorage storage)
    {
        _context = context;
        _storage = storage;
    }

    public async Task<MediaFileResult> ExecuteAsync(Guid publicId, CancellationToken ct)
    {
        var media = await _context.Medias
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == publicId, ct)
            ?? throw new NotFoundException();

        var stream = await _storage.OpenAsync(media.StorageKey, ct);

        return new MediaFileResult
        {
            Stream = stream,
            ContentType = media.ContentType,
            FileName = media.OriginalName,
        };
    }
}

public class MediaFileResult
{
    public Stream Stream { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
}