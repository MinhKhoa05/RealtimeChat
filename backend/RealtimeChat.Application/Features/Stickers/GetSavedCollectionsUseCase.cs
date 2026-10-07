using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Stickers;

public class GetSavedCollectionsUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetSavedCollectionsUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CollectionResponse>> ExecuteAsync(CancellationToken ct)
    {
        var collections = await _context.UserStickerCollections
            .AsNoTracking()
            .Where(x => x.UserId == _currentUser.UserId)
            .Select(x => x.Collection)
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

public class CollectionResponse
{
    public long Id { get; set; }
    public string Label { get; set; } = null!;
    public IReadOnlyList<StickerResponse> Stickers { get; set; } = [];
}