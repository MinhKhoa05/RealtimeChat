using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Auth;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUseCase _register;
    private readonly LoginUseCase _login;
    private readonly RefreshTokenUseCase _refreshToken;
    private readonly LogoutUseCase _logout;
    private readonly LogoutAllUseCase _logoutAll;
    private readonly ChangePasswordUseCase _changePassword;

    public AuthController(
        RegisterUseCase register,
        LoginUseCase login,
        RefreshTokenUseCase refreshToken,
        LogoutUseCase logout,
        LogoutAllUseCase logoutAll,
        ChangePasswordUseCase changePassword)
    {
        _register = register;
        _login = login;
        _refreshToken = refreshToken;
        _logout = logout;
        _logoutAll = logoutAll;
        _changePassword = changePassword;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        await _register.ExecuteAsync(request, ct);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await _login.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken ct)
    {
        var response = await _refreshToken.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        await _logout.ExecuteAsync(request, ct);
        return Ok();
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        await _logoutAll.ExecuteAsync(ct);
        return Ok();
    }

    [Authorize]
    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        await _changePassword.ExecuteAsync(request, ct);
        return Ok();
    }
}
