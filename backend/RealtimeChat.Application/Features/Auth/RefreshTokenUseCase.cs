using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
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
        var hash = _tokenService.HashRefreshToken(request.RefreshToken);
        
        var refreshToken = await _context.RefreshTokens
            .Include(x=> x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == hash, ct)
            ?? throw new Exception("Invalid Refresh Token");

        if (refreshToken.RevokedAt != null)
        {
            throw new Exception("Token was Revoked");            
        }

        if (refreshToken.ExpiredAt < DateTime.UtcNow)
        {
            throw new Exception("Token was expired");
        }

        refreshToken.RevokedAt = DateTime.UtcNow;

        var accessToken = _tokenService.GenerateAccessToken(refreshToken.User);

        var token = _tokenService.GenerateRefreshToken();
        var tokenHash = _tokenService.HashRefreshToken(token);

        var newRefreshToken = new RefreshToken
        {
            UserId = refreshToken.User.Id,
            TokenHash = tokenHash,
            ExpiredAt = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(ct);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = token
        };
    }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenResponse
{
    public string AccessToken {get; set; } = null!;
    public string RefreshToken {get; set;} = null!;
}