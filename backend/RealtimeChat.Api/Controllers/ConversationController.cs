using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Conversations;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/conversations")]
public class ConversationController : ControllerBase
{
    private readonly StartDirectConversationUseCase _startDirectConversation;
    private readonly GetConversationUseCase _getConversation;
    private readonly GetListConversationUseCase _getListConversation;
    private readonly SetConversationPinUseCase _setConversationPin;

    public ConversationController(
        StartDirectConversationUseCase startDirectConversation,
        GetConversationUseCase getConversation,
        GetListConversationUseCase getConversations,
        SetConversationPinUseCase setConversationPin)
    {
        _startDirectConversation = startDirectConversation;
        _getConversation = getConversation;
        _getListConversation = getConversations;
        _setConversationPin = setConversationPin;
    }

    [HttpPost("direct")]
    public async Task<IActionResult> StartDirectConversation(
        StartDirectConversationRequest request,
        CancellationToken ct)
    {
        var conversationId = await _startDirectConversation.ExecuteAsync(request, ct);
        return Ok(conversationId);
    }

    [HttpGet]
    public async Task<IActionResult> GetListConversation(CancellationToken ct)
    {
        var conversations = await _getListConversation.ExecuteAsync(ct);
        return Ok(conversations);
    }

    [HttpGet("{conversationId:long}")]
    public async Task<IActionResult> GetConversation(long conversationId, CancellationToken ct)
    {
        var conversation = await _getConversation.ExecuteAsync(conversationId, ct);
        return Ok(conversation);
    }

    [HttpPost("{conversationId:long}/pin")]
    public async Task<IActionResult> PinConversation(long conversationId, [FromBody] SetConversationPinRequest request, CancellationToken ct)
    {
        await _setConversationPin.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }
}