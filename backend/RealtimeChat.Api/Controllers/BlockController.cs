using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Auth;
using RealtimeChat.Application.Features.Blocks;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/blocks")]
public class BlockController : ControllerBase
{
    private readonly BlockUserUseCase _blockUserUseCase;
    private readonly UnBlockUserUseCase _unBlockUserUseCase;
    private readonly GetBlockedUsersUseCase _getBlockedUsersUseCase;


    public BlockController(BlockUserUseCase blockUserUseCase, UnBlockUserUseCase unBlockUserUseCase, GetBlockedUsersUseCase getBlockedUsersUseCase)
    {
        _blockUserUseCase = blockUserUseCase;
        _unBlockUserUseCase = unBlockUserUseCase;
        _getBlockedUsersUseCase = getBlockedUsersUseCase;
    }

    [HttpPost("{userId:long}")]
    public async Task<IActionResult> BlockUser(long userId, CancellationToken ct)
    {
        await _blockUserUseCase.ExecuteAsync(userId, ct);
        return Ok();
    }

    [HttpDelete("{userId:long}")]
    public async Task<IActionResult> UnBlockUser(long userId, CancellationToken ct)
    {
        await _unBlockUserUseCase.ExecuteAsync(userId, ct);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetBlockedUsers(CancellationToken ct)
    {
        var blockedUsers = await _getBlockedUsersUseCase.ExecuteAsync(ct);
        return Ok(blockedUsers);
    }
}