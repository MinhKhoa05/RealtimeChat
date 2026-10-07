using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Stickers;

public class RegenerateSharedKeyUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RegenerateSharedKeyUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long collectionId, CancellationToken ct)
    {
        var collection = await _context.StickerCollections.FindAsync(collectionId, ct)
            ?? throw new NotFoundException();

        if (collection.OwnerId != _currentUser.UserId)
        {
            throw new ForbiddenException();
        }

        collection.RegenerateSharedKey();

        await _context.SaveChangesAsync(ct);
    }
}