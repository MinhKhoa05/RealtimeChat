using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Auth;

public class RefreshTokenUseCase
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;

    public RefreshTokenUseCase(IAppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenResponse> ExecuteAsync(RefreshTokenRequest request, CancellationToken ct)
    {
        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == refreshTokenHash, ct)
            ?? throw new UnauthorizedException("Invalid refresh token.");

        if (!refreshToken.IsUsable())
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        refreshToken.Revoke();

        var accessToken = _tokenService.GenerateAccessToken(refreshToken.UserId);

        var rawRefreshToken = _tokenService.GenerateRefreshToken();
        refreshTokenHash = _tokenService.HashRefreshToken(rawRefreshToken);

        var newRefreshToken = RefreshToken.Create(refreshToken.UserId, refreshTokenHash);
        _context.RefreshTokens.Add(newRefreshToken);

        await _context.SaveChangesAsync(ct);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = rawRefreshToken
        };
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}