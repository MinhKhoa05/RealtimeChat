using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Blocks;

public class UnblockUserUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UnblockUserUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(long userId, CancellationToken ct)
    {
        var userBlock = await _context.Relationships
            .Blocks()
            .Where(x => x.UserId == _currentUser.UserId && x.TargetUserId == userId)
            .FirstOrDefaultAsync(ct);

        if (userBlock is not null)
        {
            _context.Relationships.Remove(userBlock);
            await _context.SaveChangesAsync(ct);
        }
    }
}