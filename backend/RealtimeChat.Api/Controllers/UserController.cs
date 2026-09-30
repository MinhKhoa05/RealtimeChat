using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Users;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly GetCurrentUserUseCase _getCurrentUser;
    private readonly GetUserProfileUseCase _getUserProfile;
    private readonly SearchUsersUseCase _searchUsers;

    public UserController(
        GetCurrentUserUseCase getCurrentUser,
        GetUserProfileUseCase getUserProfile,
        SearchUsersUseCase searchUsers)
    {
        _getCurrentUser = getCurrentUser;
        _getUserProfile = getUserProfile;
        _searchUsers = searchUsers;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var result = await _getCurrentUser.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUser([FromQuery] string keyword, CancellationToken ct)
    {
        var result = await _searchUsers.ExecuteAsync(keyword, ct);
        return Ok(result);
    }

    [HttpGet("{userId:long}")]
    public async Task<IActionResult> GetUserProfile(long userId, CancellationToken ct)
    {
        var result = await _getUserProfile.ExecuteAsync(userId, ct);
        return Ok(result);
    }
}