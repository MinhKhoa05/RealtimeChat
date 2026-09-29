using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Friends;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/friends")]
public class FriendController : ControllerBase
{
    private readonly SendFriendRequestUseCase _sendFriendRequestUseCase;
    private readonly AcceptFriendRequestUseCase _acceptFriendRequestUseCase;
    private readonly RejectFriendRequestUseCase _rejectFriendRequestUseCase;
    private readonly RevokeFriendRequestUseCase _revokeFriendRequestUseCase;

    public FriendController(SendFriendRequestUseCase sendFriendRequestUseCase, AcceptFriendRequestUseCase acceptFriendRequestUseCase,
        RejectFriendRequestUseCase rejectFriendRequestUseCase, RevokeFriendRequestUseCase revokeFriendRequestUseCase)
    {
        _sendFriendRequestUseCase = sendFriendRequestUseCase;
        _acceptFriendRequestUseCase = acceptFriendRequestUseCase;
        _rejectFriendRequestUseCase = rejectFriendRequestUseCase;
        _revokeFriendRequestUseCase = revokeFriendRequestUseCase;
    }

    [HttpPost("requests")]
    public async Task<IActionResult> SendFriendRequest(SendFriendRequestRequest request, CancellationToken ct)
    {
        await _sendFriendRequestUseCase.ExecuteAsync(request, ct);
        return Ok();
    }

    [HttpPost("requests/{requestId:long}/accepts")]
    public async Task<IActionResult> AcceptFriendRequest(long requestId, CancellationToken ct)
    {
        await _acceptFriendRequestUseCase.ExecuteAsync(requestId, ct);
        return Ok();
    }

    [HttpPost("requests/{requestId:long}/reject")]
    public async Task<IActionResult> RejectFriendRequest(long requestId, CancellationToken ct)
    {
        await _rejectFriendRequestUseCase.ExecuteAsync(requestId, ct);
        return Ok();
    }

    [HttpPost("requests/{requestId:long}/revoke")]
    public async Task<IActionResult> RevokeFriendRequest(long requestId, CancellationToken ct)
    {
        await _revokeFriendRequestUseCase.ExecuteAsync(requestId, ct);
        return Ok();
    }
}