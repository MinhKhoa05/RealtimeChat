using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Common.Pagination;
using RealtimeChat.Application.Features.Messages;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/conversations/{conversationId:long}/messages")]
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

    /// <summary>
    /// Gửi tin nhắn dạng văn bản
    /// </summary>
    [HttpPost("text")]
    public async Task<IActionResult> SendTextMessage(long conversationId, SendTextMessageRequest request, CancellationToken ct)
    {
        await _sendTextMessage.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    /// <summary>
    /// Gửi tin nhắn chứa đa phương tiện (ảnh, video, file)
    /// </summary>
    [HttpPost("media")]
    public async Task<IActionResult> SendMediaMessage(long conversationId, SendMediaMessageRequest request, CancellationToken ct)
    {
        await _sendMediaMessage.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    /// <summary>
    /// Gửi tin nhắn nhãn dán (sticker)
    /// </summary>
    [HttpPost("sticker")]
    public async Task<IActionResult> SendStickerMessage(long conversationId, SendStickerMessageRequest request, CancellationToken ct)
    {
        await _sendStickerMessage.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    /// <summary>
    /// Thu hồi tin nhắn
    /// </summary>
    [HttpPost("~/api/v{version:apiVersion}/messages/{messageId:long}/recall")]
    public async Task<IActionResult> RecallMessage(long messageId, CancellationToken ct)
    {
        await _recallMessage.ExecuteAsync(messageId, ct);
        return Ok();
    }

    /// <summary>
    /// Lấy danh sách tin nhắn theo phân trang con trỏ (cursor pagination)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CursorPaginationResult<MessageResponse>>> GetMessages(long conversationId, [FromQuery] CursorPaginationQuery request, CancellationToken ct)
    {
        var result = await _getMessages.ExecuteAsync(conversationId, request, ct);
        return Ok(result);
    }
}