using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Relationships;

public class GetBlockedUsersUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetBlockedUsersUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<long>> ExecuteAsync(CancellationToken ct)
    {
        var blockedUserIds = await _context.Relationships
            .AsNoTracking()
            .Blocks()
            .Where(x => x.UserId == _currentUser.UserId)
            .Select(x => x.TargetUserId)
            .ToListAsync(ct);

        
        return blockedUserIds;
    }
}