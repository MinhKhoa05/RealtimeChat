using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Calls;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/calls")]
public class CallsController : ControllerBase
{
    private readonly CreateCallUseCase _createCall;
    private readonly AcceptCallUseCase _acceptCall;
    private readonly CancelledCallUseCase _cancelCall;
    private readonly RejectedCallUseCase _rejectCall;
    private readonly EndCallUseCase _endCall;

    public CallsController(
        CreateCallUseCase createCall,
        AcceptCallUseCase acceptCall,
        CancelledCallUseCase cancelCall,
        RejectedCallUseCase rejectCall,
        EndCallUseCase endCall)
    {
        _createCall = createCall;
        _acceptCall = acceptCall;
        _cancelCall = cancelCall;
        _rejectCall = rejectCall;
        _endCall = endCall;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCallRequest request, CancellationToken ct)
    {
        var callId = await _createCall.ExecuteAsync(request.ReceiverId, ct);
        return Ok(callId);
    }

    [HttpPost("{callId:long}/accept")]
    public async Task<IActionResult> Accept(long callId, CancellationToken ct)
    {
        await _acceptCall.ExecuteAsync(callId, ct);
        return NoContent();
    }

    [HttpPost("{callId:long}/cancel")]
    public async Task<IActionResult> Cancel(long callId, CancellationToken ct)
    {
        await _cancelCall.ExecuteAsync(callId, ct);
        return NoContent();
    }

    [HttpPost("{callId:long}/reject")]
    public async Task<IActionResult> Reject(long callId, CancellationToken ct)
    {
        await _rejectCall.ExecuteAsync(callId, ct);
        return NoContent();
    }

    [HttpPost("{callId:long}/end")]
    public async Task<IActionResult> End(long callId, CancellationToken ct)
    {
        await _endCall.ExecuteAsync(callId, ct);
        return NoContent();
    }
}

public record CreateCallRequest(long ReceiverId);
