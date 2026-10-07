using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Stickers;

public class GetMyCollectionsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetMyCollectionsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CollectionResponse>> ExecuteAsync(CancellationToken ct)
    {
        var collections = await _context.StickerCollections
            .AsNoTracking()
            .Where(x => x.OwnerId == _currentUser.UserId)
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