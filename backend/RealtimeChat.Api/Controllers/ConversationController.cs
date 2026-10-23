
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Conversations;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/conversations")]
public class ConversationController : ControllerBase
{
    private readonly OpenDirectConversationUseCase _openDirectConversation;
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
        OpenDirectConversationUseCase openDirectConversation,
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
        _openDirectConversation = openDirectConversation;
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

    /// <summary>
    /// Mở hoặc bắt đầu cuộc trò chuyện trực tiếp (1-1) với người dùng khác
    /// </summary>
    [HttpGet("direct/{otherUserId:long}")]
    public async Task<ActionResult<long>> StartDirectConversation(long otherUserId, CancellationToken ct)
    {
        var id = await _openDirectConversation.ExecuteAsync(otherUserId, ct);
        return Ok(id);
    }

    /// <summary>
    /// Lấy danh sách cuộc trò chuyện của người dùng hiện tại
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ConversationItem>>> GetConversations(CancellationToken ct)
    {
        // Không lọc type (lấy tất cả conversation của user
        var result = await _getConversations.ExecuteAsync(type: null, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách các nhóm trò chuyện của người dùng hiện tại
    /// </summary>
    [HttpGet("groups")]
    public async Task<ActionResult<List<ConversationItem>>> GetGroups(CancellationToken ct)
    {
        var result = await _getConversations.ExecuteAsync(ConversationType.Group, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết cuộc trò chuyện
    /// </summary>
    [HttpGet("{conversationId:long}")]
    public async Task<ActionResult<ConversationResponse>> GetConversation(long conversationId, CancellationToken ct)
    {
        var result = await _getConversation.ExecuteAsync(conversationId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Ghim hoặc bỏ ghim cuộc trò chuyện
    /// </summary>
    [HttpPost("{conversationId:long}/pin")]
    public async Task<IActionResult> PinConversation(long conversationId, [FromBody] SetConversationPinRequest request, CancellationToken ct)
    {
        await _setConversationPin.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    // Groups

    /// <summary>
    /// Tạo nhóm trò chuyện mới
    /// </summary>
    [HttpPost("groups")]
    public async Task<ActionResult<long>> CreateGroup([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var id = await _createGroup.ExecuteAsync(request, ct);
        return Ok(id);
    }

    /// <summary>
    /// Lấy danh sách thành viên trong nhóm
    /// </summary>
    [HttpGet("groups/{groupId:long}/members")]
    public async Task<ActionResult<List<GroupMemberResponse>>> GetGroupMembers(long groupId, CancellationToken ct)
    {
        var result = await _getGroupMembers.ExecuteAsync(groupId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Thêm thành viên vào nhóm
    /// </summary>
    [HttpPost("groups/{groupId:long}/members/{userId:long}")]
    public async Task<IActionResult> AddMember(long groupId, long userId, CancellationToken ct)
    {
        await _addMember.ExecuteAsync(groupId, userId, ct);
        return NoContent();
    }

    /// <summary>
    /// Xóa thành viên khỏi nhóm
    /// </summary>
    [HttpDelete("groups/{groupId:long}/members/{userId:long}")]
    public async Task<IActionResult> KickMember(long groupId, long userId, CancellationToken ct)
    {
        await _kickMember.ExecuteAsync(groupId, userId, ct);
        return NoContent();
    }

    /// <summary>
    /// Rời khỏi nhóm trò chuyện
    /// </summary>
    [HttpPost("groups/{groupId:long}/leave")]
    public async Task<IActionResult> LeaveGroup(long groupId, CancellationToken ct)
    {
        await _leaveGroup.ExecuteAsync(groupId, ct);
        return NoContent();
    }

    /// <summary>
    /// Chuyển quyền quản trị viên nhóm cho thành viên khác
    /// </summary>
    [HttpPatch("groups/{groupId:long}/admin")]
    public async Task<IActionResult> TransferAdmin(long groupId, [FromBody] TransferAdminRequest request, CancellationToken ct)
    {
        await _transferAdmin.ExecuteAsync(groupId, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Tham gia nhóm trò chuyện bằng mã mời (invite code)
    /// </summary>
    [HttpPost("groups/join/{inviteCode:guid}")]
    public async Task<IActionResult> JoinGroupByInvite(Guid inviteCode, CancellationToken ct)
    {
        await _joinGroupByInvite.ExecuteAsync(inviteCode, ct);
        return NoContent();
    }

    /// <summary>
    /// Lấy thông tin nhóm bằng mã mời (invite code)
    /// </summary>
    [HttpGet("invites/{inviteCode:guid}")]
    public async Task<ActionResult<GroupInviteResponse>> GetGroupByInviteCode(Guid inviteCode, CancellationToken ct)
    {
        var result = await _getGroupByInviteCode.ExecuteAsync(inviteCode, ct);
        return Ok(result);
    }
}
