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

    public MessageController(
        SendMessageUseCase sendMessage)
    {
        _sendMessage = sendMessage;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(long conversationId, SendMessageRequest request, CancellationToken ct)
    {
        await _sendMessage.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }
}