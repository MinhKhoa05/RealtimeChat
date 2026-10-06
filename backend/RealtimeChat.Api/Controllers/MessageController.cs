using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Messages;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/conversations/{conversationId:long}/messages")]
public class MessageController : ControllerBase
{
    private readonly SendMessageUseCase _sendMessage;
    private readonly RecallMessageUseCase _recallMessage;

    public MessageController(
        SendMessageUseCase sendMessage,
        RecallMessageUseCase recallMessage)
    {
        _sendMessage = sendMessage;
        _recallMessage = recallMessage;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(long conversationId, SendMessageRequest request, CancellationToken ct)
    {
        await _sendMessage.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    [HttpPost("/api/messages/{messageId:long}/recall")]
    public async Task<IActionResult> RecallMessage(long messageId, CancellationToken ct)
    {
        await _recallMessage.ExecuteAsync(messageId, ct);
        return Ok();
    }
}