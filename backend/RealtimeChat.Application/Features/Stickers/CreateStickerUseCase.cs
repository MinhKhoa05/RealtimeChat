using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace RealtimeChat.Application.Features.Stickers;

public class CreateStickerUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateStickerUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<StickerResponse> ExecuteAsync(long collectionId, CreateStickerRequest request, CancellationToken ct)
    {
        var collection = await _context.StickerCollections.FindAsync(collectionId, ct)
            ?? throw new NotFoundException("Collection not found.");

        if (collection.OwnerId != _currentUser.UserId)
        {
            throw new ForbiddenException();
        }

        var media = await _context.Medias.FirstOrDefaultAsync(x => x.PublicId == request.MediaPublicId, ct)
            ?? throw new NotFoundException("Media not found.");

        var sticker = Sticker.Create(collection.Id, request.Label, media);

        _context.Stickers.Add(sticker);
        await _context.SaveChangesAsync(ct);

        return StickerMapper.ToResponse(sticker);
    }
}

public class CreateStickerRequest
{
    public string Label { get; set; } = null!;
    public Guid MediaPublicId { get; set; }
}