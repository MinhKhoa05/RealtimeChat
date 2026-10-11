using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;

namespace RealtimeChat.Application.Features.Users;

public class SearchUsersUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public SearchUsersUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<long>> ExecuteAsync(string keyword, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        return await _context.Users
            .AsNoTracking()
            .Where(x =>
                x.Id != currentUserId &&
                (x.Email.Contains(keyword) ||
                 x.Name.Contains(keyword)))
            .Select(x => x.Id)
            .ToListAsync(ct);
    }
}