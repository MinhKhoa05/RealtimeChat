using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Friends;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/friends")]
public class FriendController : ControllerBase
{
    private readonly SendFriendRequestUseCase _sendFriendRequest;
    private readonly AcceptFriendRequestUseCase _acceptFriendRequest;
    private readonly RejectFriendRequestUseCase _rejectFriendRequest;
    private readonly RevokeFriendRequestUseCase _revokeFriendRequest;
    private readonly RemoveFriendUseCase _removeFriend;

    private readonly GetFriendsUseCase _getFriends;
    private readonly GetSentFriendRequestsUseCase _getSentFriendRequests;
    private readonly GetReceivedFriendRequestsUseCase _getReceivedFriendRequests;

    public FriendController(
        SendFriendRequestUseCase sendFriendRequest,
        AcceptFriendRequestUseCase acceptFriendRequest,
        RejectFriendRequestUseCase rejectFriendRequest,
        RevokeFriendRequestUseCase revokeFriendRequest,
        RemoveFriendUseCase removeFriend,
        GetFriendsUseCase getFriends,
        GetSentFriendRequestsUseCase getSentFriendRequests,
        GetReceivedFriendRequestsUseCase getReceivedFriendRequests)
    {
        _sendFriendRequest = sendFriendRequest;
        _acceptFriendRequest = acceptFriendRequest;
        _rejectFriendRequest = rejectFriendRequest;
        _revokeFriendRequest = revokeFriendRequest;
        _removeFriend = removeFriend;

        _getFriends = getFriends;
        _getSentFriendRequests = getSentFriendRequests;
        _getReceivedFriendRequests = getReceivedFriendRequests;
    }

    [HttpPost("requests")]
    public async Task<IActionResult> SendFriendRequest(SendFriendRequestRequest request, CancellationToken ct)
    {
        await _sendFriendRequest.ExecuteAsync(request, ct);
        return Ok();
    }

    [HttpPost("requests/{requestId:long}/accept")]
    public async Task<IActionResult> AcceptFriendRequest(long requestId, CancellationToken ct)
    {
        await _acceptFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    [HttpPost("requests/{requestId:long}/reject")]
    public async Task<IActionResult> RejectFriendRequest(long requestId, CancellationToken ct)
    {
        await _rejectFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    [HttpPost("requests/{requestId:long}/revoke")]
    public async Task<IActionResult> RevokeFriendRequest(long requestId, CancellationToken ct)
    {
        await _revokeFriendRequest.ExecuteAsync(requestId, ct);
        return Ok();
    }

    [HttpDelete("{userId:long}")]
    public async Task<IActionResult> RemoveFriend(long userId, CancellationToken ct)
    {
        await _removeFriend.ExecuteAsync(userId, ct);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetFriends(CancellationToken ct)
    {
        var friends = await _getFriends.ExecuteAsync(ct);
        return Ok(friends);
    }

    [HttpGet("requests/sent")]
    public async Task<IActionResult> GetSentFriendRequests(CancellationToken ct)
    {
        var friendRequests = await _getSentFriendRequests.ExecuteAsync(ct);
        return Ok(friendRequests);
    }

    [HttpGet("requests/received")]
    public async Task<IActionResult> GetReceivedFriendRequests(CancellationToken ct)
    {
        var friendRequests = await _getReceivedFriendRequests.ExecuteAsync(ct);
        return Ok(friendRequests);
    }
}