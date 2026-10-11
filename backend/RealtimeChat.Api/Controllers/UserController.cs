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
    private readonly GetUserDetailUseCase _getUserDetail;
    private readonly GetUserSummariesUseCase _getUserSummaries;
    private readonly SearchUsersUseCase _searchUsers;

    public UserController(
        GetCurrentUserUseCase getCurrentUser,
        GetUserDetailUseCase getUserDetail,
        GetUserSummariesUseCase getUserSummaries,
        SearchUsersUseCase searchUsers)
    {
        _getCurrentUser = getCurrentUser;
        _getUserDetail = getUserDetail;
        _getUserSummaries = getUserSummaries;
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
    public async Task<IActionResult> GetUserDetail(long userId, CancellationToken ct)
    {
        var result = await _getUserDetail.ExecuteAsync(userId, ct);
        return Ok(result);
    }

    [HttpGet("batch")]
    public async Task<IActionResult> GetUserSummaries(IReadOnlyList<long> userIds, CancellationToken ct)
    {
        var result = await _getUserSummaries.ExecuteAsync(userIds, ct);
        return Ok(result);
    }
}