
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Conversations;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/conversations")]
public class ConversationController : ControllerBase
{
    private readonly StartDirectConversationUseCase _startDirectConversation;
    private readonly GetConversationUseCase _getConversation;
    private readonly GetConversationsUseCase _getConversations;
    private readonly SetConversationPinUseCase _setConversationPin;

    private readonly AddMemberUseCase _addMember;
    private readonly CreateGroupUseCase _createGroup;
    private readonly GetGroupMembersUseCase _getGroupMembers;
    private readonly KickMemberUseCase _kickMember;
    private readonly LeaveGroupUseCase _leaveGroup;
    private readonly TransferAdminUseCase _transferAdmin;
    private readonly DisbandGroupUseCase _disbandGroup;
    private readonly JoinGroupByInviteUseCase _joinGroupByInvite;
    private readonly GetGroupByInviteCodeUseCase _getGroupByInviteCode;

    public ConversationController(
        StartDirectConversationUseCase startDirectConversation,
        GetConversationUseCase getConversation,
        GetConversationsUseCase getConversations,
        SetConversationPinUseCase setConversationPin,
        AddMemberUseCase addMember,
        CreateGroupUseCase createGroup,
        GetGroupMembersUseCase getGroupMembers,
        KickMemberUseCase kickMember,
        LeaveGroupUseCase leaveGroup,
        TransferAdminUseCase transferAdmin,
        DisbandGroupUseCase disbandGroup,
        JoinGroupByInviteUseCase joinGroupByInvite,
        GetGroupByInviteCodeUseCase getGroupByInviteCode)
    {
        _startDirectConversation = startDirectConversation;
        _getConversation = getConversation;
        _getConversations = getConversations;
        _setConversationPin = setConversationPin;

        _addMember = addMember;
        _createGroup = createGroup;
        _getGroupMembers = getGroupMembers;
        _kickMember = kickMember;
        _leaveGroup = leaveGroup;
        _transferAdmin = transferAdmin;
        _disbandGroup = disbandGroup;
        _joinGroupByInvite = joinGroupByInvite;
        _getGroupByInviteCode = getGroupByInviteCode;
    }

    // Conversations

    [HttpPost("direct")]
    public async Task<IActionResult> StartDirectConversation([FromBody] StartDirectConversationRequest request, CancellationToken ct)
    {
        var id = await _startDirectConversation.ExecuteAsync(request, ct);
        return Ok(id);
    }

    [HttpGet]
    public async Task<IActionResult> GetConversations(CancellationToken ct)
    {
        // Không lọc type (lấy tất cả conversation của user
        var result = await _getConversations.ExecuteAsync(type: null, ct);
        return Ok(result);
    }

    [HttpGet("groups")]
    public async Task<IActionResult> GetGroups(CancellationToken ct)
    {
        var result = await _getConversations.ExecuteAsync(ConversationType.Group, ct);
        return Ok(result);
    }

    [HttpGet("{conversationId:long}")]
    public async Task<IActionResult> GetConversation(long conversationId, CancellationToken ct)
    {
        var result = await _getConversation.ExecuteAsync(conversationId, ct);
        return Ok(result);
    }

    [HttpPost("{conversationId:long}/pin")]
    public async Task<IActionResult> PinConversation(long conversationId, [FromBody] SetConversationPinRequest request, CancellationToken ct)
    {
        await _setConversationPin.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    // Groups

    [HttpPost("groups")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var id = await _createGroup.ExecuteAsync(request, ct);
        return Ok(id);
    }

    [HttpGet("groups/{groupId:long}/members")]
    public async Task<IActionResult> GetGroupMembers(long groupId, CancellationToken ct)
    {
        var result = await _getGroupMembers.ExecuteAsync(groupId, ct);
        return Ok(result);
    }

    [HttpPost("groups/{groupId:long}/members/{userId:long}")]
    public async Task<IActionResult> AddMember(long groupId, long userId, CancellationToken ct)
    {
        await _addMember.ExecuteAsync(groupId, userId, ct);
        return NoContent();
    }

    [HttpDelete("groups/{groupId:long}/members/{userId:long}")]
    public async Task<IActionResult> KickMember(long groupId, long userId, CancellationToken ct)
    {
        await _kickMember.ExecuteAsync(groupId, userId, ct);
        return NoContent();
    }

    [HttpPost("groups/{groupId:long}/leave")]
    public async Task<IActionResult> LeaveGroup(long groupId, CancellationToken ct)
    {
        await _leaveGroup.ExecuteAsync(groupId, ct);
        return NoContent();
    }

    [HttpPatch("groups/{groupId:long}/admin")]
    public async Task<IActionResult> TransferAdmin(long groupId, [FromBody] TransferAdminRequest request, CancellationToken ct)
    {
        await _transferAdmin.ExecuteAsync(groupId, request, ct);
        return NoContent();
    }

    [HttpPost("groups/join/{inviteCode:guid}")]
    public async Task<IActionResult> JoinGroupByInvite(Guid inviteCode, CancellationToken ct)
    {
        await _joinGroupByInvite.ExecuteAsync(inviteCode, ct);
        return NoContent();
    }

    [HttpGet("invites/{inviteCode:guid}")]
    public async Task<IActionResult> GetGroupByInviteCode(Guid inviteCode, CancellationToken ct)
    {
        var result = await _getGroupByInviteCode.ExecuteAsync(inviteCode, ct);
        return Ok(result);
    }
}
