using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Blocks;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/blocks")]
public class BlockController : ControllerBase
{
    private readonly BlockUserUseCase _blockUser;
    private readonly UnblockUserUseCase _unblockUser;
    private readonly GetBlockedUsersUseCase _getBlockedUsers;

    public BlockController(
        BlockUserUseCase blockUser,
        UnblockUserUseCase unblockUser,
        GetBlockedUsersUseCase getBlockedUsers)
    {
        _blockUser = blockUser;
        _unblockUser = unblockUser;
        _getBlockedUsers = getBlockedUsers;
    }

    [HttpPost("{userId:long}")]
    public async Task<IActionResult> BlockUser(long userId, CancellationToken ct)
    {
        await _blockUser.ExecuteAsync(userId, ct);
        return Ok();
    }

    [HttpDelete("{userId:long}")]
    public async Task<IActionResult> UnblockUser(long userId, CancellationToken ct)
    {
        await _unblockUser.ExecuteAsync(userId, ct);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetBlockedUsers(CancellationToken ct)
    {
        var blockedUsers = await _getBlockedUsers.ExecuteAsync(ct);
        return Ok(blockedUsers);
    }
}
