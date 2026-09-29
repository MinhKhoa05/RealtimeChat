using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Auth;

public class LogoutAllUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;

    public LogoutAllUseCase(IAppDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {

        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == _currentUser.UserId && x.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
    }
}