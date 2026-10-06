using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Common.Pagination;
using RealtimeChat.Application.Features.Messages;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/conversations/{conversationId:long}/messages")]
public class MessageController : ControllerBase
{
    private readonly SendMessageUseCase _sendMessage;
    private readonly RecallMessageUseCase _recallMessage;
    private readonly GetMessagesUseCase _getMessages;

    public MessageController(
        SendMessageUseCase sendMessage,
        RecallMessageUseCase recallMessage,
        GetMessagesUseCase getMessages)
    {
        _sendMessage = sendMessage;
        _recallMessage = recallMessage;
        _getMessages = getMessages;
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

    [HttpGet]
    public async Task<IActionResult> GetMessages(long conversationId, [FromQuery] CursorPaginationQuery request, CancellationToken ct)
    {
        var result = await _getMessages.ExecuteAsync(conversationId, request, ct);
        return Ok(result);
    }
}