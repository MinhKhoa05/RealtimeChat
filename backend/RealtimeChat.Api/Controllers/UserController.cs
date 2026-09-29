using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Users;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetCurrentUserUseCase _getCurrentUserUseCase;
    private readonly GetUserProfileUseCase _getUserProfileUseCase;

    public UserController(GetCurrentUserUseCase getCurrentUserUseCase, GetUserProfileUseCase getUserProfileUseCase)
    {
        _getCurrentUserUseCase = getCurrentUserUseCase;
        _getUserProfileUseCase = getUserProfileUseCase;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var response = await _getCurrentUserUseCase.ExecuteAsync(ct);
        return Ok(response);
    }

    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetUserProfile(long userId, CancellationToken ct)
    {
        var response = await _getUserProfileUseCase.ExecuteAsync(userId, ct);
        return Ok(response);
    }
}