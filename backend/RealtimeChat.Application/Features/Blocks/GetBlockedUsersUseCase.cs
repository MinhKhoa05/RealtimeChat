using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Blocks;

public class GetBlockedUsersUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetBlockedUsersUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<BlockedUserResponse>> ExecuteAsync(CancellationToken ct)
    {
        var blockedUsers = await _context.Relationships
            .AsNoTracking()
            .Blocks()
            .Where(x => x.UserId == _currentUser.UserId)
            .Select(x => new BlockedUserResponse
            {
                UserId = x.TargetUserId,
                Name = x.TargetUser.Name,
                AvatarUrl = null
            })
            .ToListAsync(ct);
        
        return blockedUsers;
    }
}

public class BlockedUserResponse
{
    public long UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? AvatarUrl { get; set; }
}