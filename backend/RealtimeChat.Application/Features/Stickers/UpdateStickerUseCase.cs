using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Stickers;

public class UpdateStickerUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateStickerUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(Guid stickerPublicId, UpdateStickerRequest request, CancellationToken ct)
    {
        var sticker = await _context.Stickers
            .FirstOrDefaultAsync(x => x.PublicId == stickerPublicId && x.Collection.OwnerId == _currentUser.UserId, ct)
            ?? throw new NotFoundException();

        sticker.UpdateLabel(request.Label);

        await _context.SaveChangesAsync(ct);
    }
}

public class UpdateStickerRequest
{
    public string Label { get; set; } = null!;
}