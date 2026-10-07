using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Stickers;

public class GetAvailableCollectionsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetAvailableCollectionsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CollectionResponse>> ExecuteAsync(CancellationToken ct)
    {
        var owned = _context.StickerCollections
            .Where(x => x.OwnerId == _currentUser.UserId);

        var saved = _context.UserStickerCollections
            .Where(x => x.UserId == _currentUser.UserId)
            .Select(x => x.Collection);

        var collections = await owned
            .Concat(saved)
            .Include(x => x.Stickers)
                .ThenInclude(x => x.Media)
            .ToListAsync(ct);

        return collections
            .Select(x => new CollectionResponse
            {
                Id = x.Id,
                Label = x.Label,
                Stickers = x.Stickers
                    .Select(StickerMapper.ToResponse)
                    .ToList()
            })
            .ToList();
    }
}