using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Blocks;

public class UnBlockUserUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UnBlockUserUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long userId, CancellationToken ct)
    {
        var userBlock = await _context.Relationships
            .FirstOrDefaultAsync(x => x.Type == Domain.Enums.RelationshipType.Block && x.UserId == _currentUser.UserId && x.TargetUserId == userId, ct);

        if (userBlock is not null)
        {
            _context.Relationships.Remove(userBlock);
            await _context.SaveChangesAsync(ct);
        }
    }
}