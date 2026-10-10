using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Auth;

public class LogoutUseCase
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public LogoutUseCase(IAppDbContext context, ITokenService tokenService, ICurrentUser currentUser, TimeProvider timeProvider)
    {
        _context = context;
        _tokenService = tokenService;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(LogoutRequest request, CancellationToken ct)
    {
        var hash = _tokenService.HashRefreshToken(request.RefreshToken);
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == hash && x.UserId == _currentUser.UserId, ct);

        if (refreshToken is null)
        {
            return;
        }

        if (!refreshToken.IsRevoked())
        {
            refreshToken.Revoke(_timeProvider.GetUtcNow().UtcDateTime);
            await _context.SaveChangesAsync(ct);
        }
    }

}

public class LogoutRequest
{
    public string RefreshToken { get; set; } = null!;
}