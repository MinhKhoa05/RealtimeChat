using RealtimeChat.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RealtimeChat.Application.Features.Users;

public class GetUserSummariesUseCase
{
    private readonly IAppDbContext _context;

    public GetUserSummariesUseCase(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserSummaryResponse>> ExecuteAsync(IReadOnlyCollection<long> userIds, CancellationToken ct)
    {
        var ids = userIds.Where(x => x > 0).Distinct().ToArray();

        if (ids.Length == 0) return [];

        return await _context.Users
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new UserSummaryResponse
            {
                Id = x.Id,
                Name = x.Name,
                AvatarUrl = null
            })
            .ToListAsync(ct);
    }
}

public class UserSummaryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
}