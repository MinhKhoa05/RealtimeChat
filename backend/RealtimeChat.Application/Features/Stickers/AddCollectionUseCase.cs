using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Stickers;

public class AddCollectionUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AddCollectionUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(Guid sharedKey, CancellationToken ct)
    {
        var collection = await _context.StickerCollections.FirstOrDefaultAsync(x => x.SharedKey == sharedKey, ct)
            ?? throw new NotFoundException();

        var alreadyAdded = await _context.UserStickerCollections
            .AnyAsync(x => x.CollectionId == collection.Id && x.UserId == _currentUser.UserId, ct);
        
        if (alreadyAdded)
        {
            throw new ConflictException("Collection already added.");
        }

        var userCollection = UserStickerCollection.Create(_currentUser.UserId, collection.Id);

        _context.UserStickerCollections.Add(userCollection);
        await _context.SaveChangesAsync(ct);
    }
}