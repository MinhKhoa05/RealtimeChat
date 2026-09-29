using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Auth;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUseCase _registerUseCase;
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly LogoutAllUseCase _logoutAllUseCase;
    private readonly ChangePasswordUseCase _changePasswordUseCase;

    public AuthController(RegisterUseCase registerUseCase, LoginUseCase loginUseCase, RefreshTokenUseCase refreshTokenUseCase,
        LogoutUseCase logoutUseCase, LogoutAllUseCase logoutAllUseCase, ChangePasswordUseCase changePasswordUseCase)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _logoutAllUseCase = logoutAllUseCase;
        _changePasswordUseCase = changePasswordUseCase;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        await _registerUseCase.ExecuteAsync(request, ct);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await _loginUseCase.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken ct)
    {
        var response = await _refreshTokenUseCase.ExecuteAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        await _logoutUseCase.ExecuteAsync(request, ct);
        return Ok();
    }

    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        await _logoutAllUseCase.ExecuteAsync(ct);
        return Ok();
    }

    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        await _changePasswordUseCase.ExecuteAsync(request, ct);
        return Ok();
    }
}