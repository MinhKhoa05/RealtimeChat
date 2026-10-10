using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.ShareTrip;
using RealtimeChat.Application.Features.ShareTrip.Services;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/share-trips")]
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

    [HttpPost("conversations/{conversationId:long}/share-trips")]
    public async Task<IActionResult> Create(long conversationId, [FromBody] CreateShareTripRequest request, CancellationToken ct)
    {
        await _createShareTrip.ExecuteAsync(conversationId, request, ct);
        return Ok();
    }

    [HttpPost("{shareTripId:long}/end")]
    public async Task<IActionResult> End(long shareTripId, CancellationToken ct)
    {
        await _endShareTrip.ExecuteAsync(shareTripId, ct);
        return NoContent();
    }

    [HttpPut("me/location")]
    public async Task<IActionResult> UpdateLiveLocation([FromBody] LocationData data, CancellationToken ct)
    {
        await _updateUserLiveLocation.ExecuteAsync(data, ct);
        return NoContent();
    }

    [HttpGet("conversations/{conversationId:long}/share-trips")]
    public async Task<IActionResult> GetConversationShareTrips(long conversationId, CancellationToken ct)
    {
        var result = await _getConversationShareTrips.ExecuteAsync(conversationId, ct);
        return Ok(result);
    }

    [HttpGet("{shareTripId:long}")]
    public async Task<IActionResult> GetById(long shareTripId, CancellationToken ct)
    {
        var result = await _getShareTripById.ExecuteAsync(shareTripId, ct);
        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetUserShareTrips(CancellationToken ct)
    {
        var result = await _getUserShareTrips.ExecuteAsync(ct);
        return Ok(result);
    }
}