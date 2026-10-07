using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Stickers;

public class UpdateCollectionUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateCollectionUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long collectionId, UpdateCollectionRequest request, CancellationToken ct)
    {
        var collection = await _context.StickerCollections.FindAsync(collectionId, ct)
            ?? throw new NotFoundException();

        if (collection.OwnerId != _currentUser.UserId)
        {
            throw new ForbiddenException();
        }

        if (request.Label is not null)
        {
            collection.UpdateLabel(request.Label);
        }

        if (request.Visibility is not null)
        {
            collection.SetVisibility(request.Visibility.Value);
        }

        await _context.SaveChangesAsync(ct);
    }
}

public class UpdateCollectionRequest
{
    public string? Label { get; set; } = null!;
    public StickerCollectionVisibility? Visibility { get; set; }
}