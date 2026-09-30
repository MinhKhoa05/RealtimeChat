using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Users;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetCurrentUserUseCase _getCurrentUserUseCase;
    private readonly GetUserProfileUseCase _getUserProfileUseCase;
    private readonly SearchUsersUseCase _searchUsersUseCase;

    public UserController(GetCurrentUserUseCase getCurrentUserUseCase, GetUserProfileUseCase getUserProfileUseCase,
        SearchUsersUseCase searchUsersUseCase)
    {
        _getCurrentUserUseCase = getCurrentUserUseCase;
        _getUserProfileUseCase = getUserProfileUseCase;
        _searchUsersUseCase = searchUsersUseCase;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var result = await _getCurrentUserUseCase.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUser([FromQuery] string keyword, CancellationToken ct)
    {
        var result = await _searchUsersUseCase.ExecuteAsync(keyword, ct);
        return Ok(result);
    }

    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetUserProfile(long userId, CancellationToken ct)
    {
        var result = await _getUserProfileUseCase.ExecuteAsync(userId, ct);
        return Ok(result);
    }
}