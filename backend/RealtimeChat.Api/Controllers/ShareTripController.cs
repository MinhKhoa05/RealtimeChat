using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.ShareTrip;
using RealtimeChat.Application.Features.ShareTrip.Services;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/share-trips")]
public class ShareTripsController : ControllerBase
{
    private readonly CreateShareTripUseCase _createShareTrip;
    private readonly EndShareTripUseCase _endShareTrip;
    private readonly UpdateUserLiveLocationUseCase _updateUserLiveLocation;
    private readonly GetConversationShareTripsUseCase _getConversationShareTrips;
    private readonly GetShareTripByIdUseCase _getShareTripById;
    private readonly GetUserShareTripsUseCase _getUserShareTrips;

    public ShareTripsController(
        CreateShareTripUseCase createShareTrip,
        EndShareTripUseCase endShareTrip,
        UpdateUserLiveLocationUseCase updateUserLiveLocation,
        GetConversationShareTripsUseCase getConversationShareTrips,
        GetShareTripByIdUseCase getShareTripById,
        GetUserShareTripsUseCase getUserShareTrips)
    {
        _createShareTrip = createShareTrip;
        _endShareTrip = endShareTrip;
        _updateUserLiveLocation = updateUserLiveLocation;
        _getConversationShareTrips = getConversationShareTrips;
        _getShareTripById = getShareTripById;
        _getUserShareTrips = getUserShareTrips;
    }

    /// <summary>
    /// Tạo chuyến đi chia sẻ trong cuộc trò chuyện
    /// </summary>
    [HttpPost("conversations/{conversationId:long}/share-trips")]
    public async Task<IActionResult> Create(long conversationId, [FromBody] CreateShareTripRequest request, CancellationToken ct)
    {
        await _createShareTrip.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    /// <summary>
    /// Kết thúc chuyến đi chia sẻ
    /// </summary>
    [HttpPost("{shareTripId:long}/end")]
    public async Task<IActionResult> End(long shareTripId, CancellationToken ct)
    {
        await _endShareTrip.ExecuteAsync(shareTripId, ct);
        return NoContent();
    }

    /// <summary>
    /// Cập nhật vị trí trực tiếp của người dùng hiện tại
    /// </summary>
    [HttpPut("me/location")]
    public async Task<IActionResult> UpdateLiveLocation([FromBody] LocationData data, CancellationToken ct)
    {
        await _updateUserLiveLocation.ExecuteAsync(data, ct);
        return NoContent();
    }

    /// <summary>
    /// Lấy danh sách chuyến đi chia sẻ trong cuộc trò chuyện
    /// </summary>
    [HttpGet("conversations/{conversationId:long}/share-trips")]
    public async Task<ActionResult<List<ShareTripSession>>> GetConversationShareTrips(long conversationId, CancellationToken ct)
    {
        var result = await _getConversationShareTrips.ExecuteAsync(conversationId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin chi tiết chuyến đi chia sẻ theo ID
    /// </summary>
    [HttpGet("{shareTripId:long}")]
    public async Task<ActionResult<ShareTripSession>> GetById(long shareTripId, CancellationToken ct)
    {
        var result = await _getShareTripById.ExecuteAsync(shareTripId, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách chuyến đi chia sẻ của người dùng hiện tại
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<List<ShareTripSession>>> GetUserShareTrips(CancellationToken ct)
    {
        var result = await _getUserShareTrips.ExecuteAsync(ct);
        return Ok(result);
    }
}