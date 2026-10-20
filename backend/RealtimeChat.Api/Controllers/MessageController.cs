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
    private readonly SendTextMessageUseCase _sendTextMessage;
    private readonly SendMediaMessageUseCase _sendMediaMessage;
    private readonly SendStickerMessageUseCase _sendStickerMessage;
    private readonly RecallMessageUseCase _recallMessage;
    private readonly GetMessagesUseCase _getMessages;

    public MessageController(
        SendTextMessageUseCase sendTextMessage,
        SendMediaMessageUseCase sendMediaMessage,
        SendStickerMessageUseCase sendStickerMessage,
        RecallMessageUseCase recallMessage,
        GetMessagesUseCase getMessages)
    {
        _sendTextMessage = sendTextMessage;
        _sendMediaMessage = sendMediaMessage;
        _sendStickerMessage = sendStickerMessage;
        _recallMessage = recallMessage;
        _getMessages = getMessages;
    }

    [HttpPost("text")]
    public async Task<IActionResult> SendTextMessage(long conversationId, SendTextMessageRequest request, CancellationToken ct)
    {
        await _sendTextMessage.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    [HttpPost("media")]
    public async Task<IActionResult> SendMediaMessage(long conversationId, SendMediaMessageRequest request, CancellationToken ct)
    {
        await _sendMediaMessage.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    [HttpPost("sticker")]
    public async Task<IActionResult> SendStickerMessage(long conversationId, SendStickerMessageRequest request, CancellationToken ct)
    {
        await _sendStickerMessage.ExecuteAsync(conversationId, request, ct);
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