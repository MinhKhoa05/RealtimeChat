using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Users;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
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

    /// <summary>
    /// Lấy thông tin tài khoản của người dùng hiện tại
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDetailResponse>> GetMe(CancellationToken ct)
    {
        var result = await _getCurrentUser.ExecuteAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// Tìm kiếm người dùng theo từ khóa
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<List<long>>> SearchUser([FromQuery] string keyword, CancellationToken ct)
    {
        var result = await _searchUsers.ExecuteAsync(keyword, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết người dùng theo ID
    /// </summary>
    [HttpGet("{userId:long}")]
    public async Task<ActionResult<UserDetailResponse>> GetUserDetail(long userId, CancellationToken ct)
    {
        var result = await _getUserDetail.ExecuteAsync(userId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy tóm tắt thông tin nhiều người dùng theo danh sách ID
    /// </summary>
    [HttpGet("batch")]
    public async Task<ActionResult<List<UserSummaryResponse>>> GetUserSummaries(IReadOnlyList<long> userIds, CancellationToken ct)
    {
        var result = await _getUserSummaries.ExecuteAsync(userIds, ct);
        return Ok(result);
    }
}