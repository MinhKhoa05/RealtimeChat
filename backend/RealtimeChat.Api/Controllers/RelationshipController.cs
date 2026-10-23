
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Relationships;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/relationships")]
public class RelationshipController : ControllerBase
{
    private readonly BlockUserUseCase _blockUser;
    private readonly UnblockUserUseCase _unblockUser;
    private readonly GetBlockedUsersUseCase _getBlockedUsers;

    private readonly SendFriendRequestUseCase _sendFriendRequest;
    private readonly AcceptFriendRequestUseCase _acceptFriendRequest;
    private readonly RejectFriendRequestUseCase _rejectFriendRequest;
    private readonly RevokeFriendRequestUseCase _revokeFriendRequest;
    private readonly RemoveFriendUseCase _removeFriend;

    private readonly GetFriendsUseCase _getFriends;
    private readonly GetSentFriendRequestsUseCase _getSentFriendRequests;
    private readonly GetReceivedFriendRequestsUseCase _getReceivedFriendRequests;

    public RelationshipController(
        BlockUserUseCase blockUser,
        UnblockUserUseCase unblockUser,
        GetBlockedUsersUseCase getBlockedUsers,
        SendFriendRequestUseCase sendFriendRequest,
        AcceptFriendRequestUseCase acceptFriendRequest,
        RejectFriendRequestUseCase rejectFriendRequest,
        RevokeFriendRequestUseCase revokeFriendRequest,
        RemoveFriendUseCase removeFriend,
        GetFriendsUseCase getFriends,
        GetSentFriendRequestsUseCase getSentFriendRequests,
        GetReceivedFriendRequestsUseCase getReceivedFriendRequests)
    {
        _blockUser = blockUser;
        _unblockUser = unblockUser;
        _getBlockedUsers = getBlockedUsers;

        _sendFriendRequest = sendFriendRequest;
        _acceptFriendRequest = acceptFriendRequest;
        _rejectFriendRequest = rejectFriendRequest;
        _revokeFriendRequest = revokeFriendRequest;
        _removeFriend = removeFriend;

        _getFriends = getFriends;
        _getSentFriendRequests = getSentFriendRequests;
        _getReceivedFriendRequests = getReceivedFriendRequests;
    }

    // Friends

    /// <summary>
    /// Lấy danh sách ID bạn bè
    /// </summary>
    [HttpGet("friends")]
    public async Task<ActionResult<List<long>>> GetFriends(CancellationToken ct)
    {
        var friends = await _getFriends.ExecuteAsync(ct);
        return Ok(friends);
    }

    /// <summary>
    /// Hủy kết bạn với người dùng khác
    /// </summary>
    [HttpDelete("friends/{userId:long}")]
    public async Task<IActionResult> RemoveFriend(long userId, CancellationToken ct)
    {
        await _removeFriend.ExecuteAsync(userId, ct);
        return Ok();
    }

    // Friend requests

    /// <summary>
    /// Gửi lời mời kết bạn
    /// </summary>
    [HttpPost("friend-requests")]
    public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestRequest request, CancellationToken ct)
    {
        await _sendFriendRequest.ExecuteAsync(request, ct);
        return Ok();
    }

    /// <summary>
    /// Lấy danh sách lời mời kết bạn đã gửi
    /// </summary>
    [HttpGet("friend-requests/sent")]
    public async Task<ActionResult<List<FriendRequestResponse>>> GetSentFriendRequests(CancellationToken ct)
    {
        var requests = await _getSentFriendRequests.ExecuteAsync(ct);
        return Ok(requests);
    }

    /// <summary>
    /// Lấy danh sách lời mời kết bạn đã nhận
    /// </summary>
    [HttpGet("friend-requests/received")]
    public async Task<ActionResult<List<FriendRequestResponse>>> GetReceivedFriendRequests(CancellationToken ct)
    {
        var requests = await _getReceivedFriendRequests.ExecuteAsync(ct);
        return Ok(requests);
    }

    /// <summary>
    /// Chấp nhận lời mời kết bạn
    /// </summary>
    [HttpPost("friend-requests/{requestId:long}/accept")]
    public async Task<IActionResult> AcceptFriendRequest(long requestId, CancellationToken ct)
    {
        await _acceptFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    /// <summary>
    /// Từ chối lời mời kết bạn
    /// </summary>
    [HttpPost("friend-requests/{requestId:long}/reject")]
    public async Task<IActionResult> RejectFriendRequest(long requestId, CancellationToken ct)
    {
        await _rejectFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    /// <summary>
    /// Thu hồi lời mời kết bạn đã gửi
    /// </summary>
    [HttpPost("friend-requests/{requestId:long}/revoke")]
    public async Task<IActionResult> RevokeFriendRequest(long requestId, CancellationToken ct)
    {
        await _revokeFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    // Blocks

    /// <summary>
    /// Lấy danh sách ID người dùng bị chặn
    /// </summary>
    [HttpGet("blocks")]
    public async Task<ActionResult<List<long>>> GetBlockedUsers(CancellationToken ct)
    {
        var blockedUsers = await _getBlockedUsers.ExecuteAsync(ct);
        return Ok(blockedUsers);
    }

    /// <summary>
    /// Chặn người dùng
    /// </summary>
    [HttpPost("blocks/{userId:long}")]
    public async Task<IActionResult> BlockUser(long userId, CancellationToken ct)
    {
        await _blockUser.ExecuteAsync(userId, ct);
        return Ok();
    }

    /// <summary>
    /// Bỏ chặn người dùng
    /// </summary>
    [HttpDelete("blocks/{userId:long}")]
    public async Task<IActionResult> UnblockUser(long userId, CancellationToken ct)
    {
        await _unblockUser.ExecuteAsync(userId, ct);
        return Ok();
    }
}
