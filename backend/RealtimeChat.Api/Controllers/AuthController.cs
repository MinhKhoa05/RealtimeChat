using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Auth;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
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

    /// <summary>
    /// Đăng ký tài khoản người dùng mới
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        await _register.ExecuteAsync(request, ct);
        return Ok();
    }

    /// <summary>
    /// Đăng nhập hệ thống
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await _login.ExecuteAsync(request, ct);
        return Ok(response);
    }

    /// <summary>
    /// Làm mới access token bằng refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken(RefreshTokenRequest request, CancellationToken ct)
    {
        var response = await _refreshToken.ExecuteAsync(request, ct);
        return Ok(response);
    }

    /// <summary>
    /// Đăng xuất phiên hiện tại
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        await _logout.ExecuteAsync(request, ct);
        return Ok();
    }

    /// <summary>
    /// Đăng xuất tất cả phiên hoạt động
    /// </summary>
    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken ct)
    {
        await _logoutAll.ExecuteAsync(ct);
        return Ok();
    }

    /// <summary>
    /// Đổi mật khẩu người dùng
    /// </summary>
    [Authorize]
    [HttpPost("password/change")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken ct)
    {
        await _changePassword.ExecuteAsync(request, ct);
        return Ok();
    }
}
