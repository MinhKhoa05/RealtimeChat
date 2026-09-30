using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Groups;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly AddMemberUseCase _addMember;
    private readonly CreateGroupUseCase _createGroup;
    private readonly GetGroupMembersUseCase _getGroupMembers;
    private readonly GetGroupUseCase _getGroup;
    private readonly GetMyGroupsUseCase _getMyGroups;
    private readonly KickMemberUseCase _kickMember;
    private readonly LeaveGroupUseCase _leaveGroup;
    private readonly TransferAdminUseCase _transferAdmin;

    public GroupsController(
        AddMemberUseCase addMember,
        CreateGroupUseCase createGroup,
        GetGroupMembersUseCase getGroupMembers,
        GetGroupUseCase getGroup,
        GetMyGroupsUseCase getMyGroups,
        KickMemberUseCase kickMember,
        LeaveGroupUseCase leaveGroup,
        TransferAdminUseCase transferAdmin)
    {
        _addMember = addMember;
        _createGroup = createGroup;
        _getGroupMembers = getGroupMembers;
        _getGroup = getGroup;
        _getMyGroups = getMyGroups;
        _kickMember = kickMember;
        _leaveGroup = leaveGroup;
        _transferAdmin = transferAdmin;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateGroupRequest request, CancellationToken ct)
    {
        var result = await _createGroup.ExecuteAsync(request, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyGroups(CancellationToken ct)
    {
        var result = await _getMyGroups.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("{groupId:long}")]
    public async Task<IActionResult> Get(long groupId, CancellationToken ct)
    {
        var result = await _getGroup.ExecuteAsync(groupId, ct);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("{groupId:long}/members")]
    public async Task<IActionResult> GetMembers(long groupId, CancellationToken ct)
    {
        var result = await _getGroupMembers.ExecuteAsync(groupId, ct);
        return Ok(result);
    }

    [HttpPost("{groupId:long}/members/{userId:long}")]
    public async Task<IActionResult> AddMember(long groupId, long userId, CancellationToken ct)
    {
        await _addMember.ExecuteAsync(groupId, userId, ct);
        return NoContent();
    }

    [HttpDelete("{groupId:long}/members/{userId:long}")]
    public async Task<IActionResult> KickMember(long groupId, long userId, CancellationToken ct)
    {
        await _kickMember.ExecuteAsync(groupId, userId, ct);
        return NoContent();
    }

    [HttpPost("{groupId:long}/leave")]
    public async Task<IActionResult> Leave(long groupId, CancellationToken ct)
    {
        await _leaveGroup.ExecuteAsync(groupId, ct);
        return NoContent();
    }

    [HttpPatch("{groupId:long}/admin")]
    public async Task<IActionResult> TransferAdmin(long groupId, TransferAdminRequest request, CancellationToken ct)
    {
        await _transferAdmin.ExecuteAsync(groupId, request, ct);
        return NoContent();
    }
}