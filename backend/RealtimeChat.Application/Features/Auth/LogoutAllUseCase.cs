using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.Auth;

public class LogoutAllUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public LogoutAllUseCase(IAppDbContext context, ICurrentUser currentUser, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        var refreshTokens = await _context.RefreshTokens
            .Where(x => x.UserId == _currentUser.UserId && x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var refreshToken in refreshTokens)
        {
            refreshToken.Revoke(_timeProvider.GetUtcNow().UtcDateTime);
        }

        await _context.SaveChangesAsync(ct);
    }
}