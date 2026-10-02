using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.Auth;

public class LoginUseCase
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUseCase(IAppDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email, ct)
            ?? throw new UnauthorizedException("Invalid Credentials");

        var isMatch = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isMatch)
        {
            throw new UnauthorizedException("Invalid Credentials");
        }

        var token = _tokenService.GenerateRefreshToken();
        var tokenHash = _tokenService.HashRefreshToken(token);

        var refreshToken = RefreshToken.Create(user.Id, tokenHash);
        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(ct);

        var accessToken = _tokenService.GenerateAccessToken(user.Id);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = token
        };
    }
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}