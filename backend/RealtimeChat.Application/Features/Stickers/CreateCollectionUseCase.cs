using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Stickers;

public class CreateCollectionUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public CreateCollectionUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CreateCollectionResponse> ExecuteAsync(CreateCollectionRequest request, CancellationToken ct)
    {
        var collection = StickerCollection.Create(_currentUser.UserId, request.Label);

        _context.StickerCollections.Add(collection);
        await _context.SaveChangesAsync(ct);

        return new CreateCollectionResponse
        {
            Id = collection.Id,
            Label = collection.Label,
            SharedKey = collection.SharedKey,
        };
    }
}

public class CreateCollectionRequest
{
    public string Label { get; set; } = null!;
}

public class CreateCollectionResponse
{
    public long Id { get; set; }
    public string Label { get; set; } = null!;
    public Guid SharedKey { get; set; }
}