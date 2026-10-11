
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Relationships;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/relationships")]
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

    [HttpGet("friends")]
    public async Task<IActionResult> GetFriends(CancellationToken ct)
    {
        var friends = await _getFriends.ExecuteAsync(ct);
        return Ok(friends);
    }

    [HttpDelete("friends/{userId:long}")]
    public async Task<IActionResult> RemoveFriend(long userId, CancellationToken ct)
    {
        await _removeFriend.ExecuteAsync(userId, ct);
        return Ok();
    }

    // Friend requests

    [HttpPost("friend-requests")]
    public async Task<IActionResult> SendFriendRequest([FromBody] SendFriendRequestRequest request, CancellationToken ct)
    {
        await _sendFriendRequest.ExecuteAsync(request, ct);
        return Ok();
    }

    [HttpGet("friend-requests/sent")]
    public async Task<IActionResult> GetSentFriendRequests(CancellationToken ct)
    {
        var requests = await _getSentFriendRequests.ExecuteAsync(ct);
        return Ok(requests);
    }

    [HttpGet("friend-requests/received")]
    public async Task<IActionResult> GetReceivedFriendRequests(CancellationToken ct)
    {
        var requests = await _getReceivedFriendRequests.ExecuteAsync(ct);
        return Ok(requests);
    }

    [HttpPost("friend-requests/{requestId:long}/accept")]
    public async Task<IActionResult> AcceptFriendRequest(long requestId, CancellationToken ct)
    {
        await _acceptFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    [HttpPost("friend-requests/{requestId:long}/reject")]
    public async Task<IActionResult> RejectFriendRequest(long requestId, CancellationToken ct)
    {
        await _rejectFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    [HttpPost("friend-requests/{requestId:long}/revoke")]
    public async Task<IActionResult> RevokeFriendRequest(long requestId, CancellationToken ct)
    {
        await _revokeFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    // Blocks

    [HttpGet("blocks")]
    public async Task<IActionResult> GetBlockedUsers(CancellationToken ct)
    {
        var blockedUsers = await _getBlockedUsers.ExecuteAsync(ct);
        return Ok(blockedUsers);
    }

    [HttpPost("blocks/{userId:long}")]
    public async Task<IActionResult> BlockUser(long userId, CancellationToken ct)
    {
        await _blockUser.ExecuteAsync(userId, ct);
        return Ok();
    }

    [HttpDelete("blocks/{userId:long}")]
    public async Task<IActionResult> UnblockUser(long userId, CancellationToken ct)
    {
        await _unblockUser.ExecuteAsync(userId, ct);
        return Ok();
    }
}
