using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Stickers;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sticker-collections")]
public class StickerCollectionsController : ControllerBase
{
    private readonly AddCollectionUseCase _addCollection;
    private readonly CreateCollectionUseCase _createCollection;
    private readonly CreateStickerUseCase _createSticker;
    private readonly GetAvailableCollectionsUseCase _getAvailable;
    private readonly GetMyCollectionsUseCase _getMy;
    private readonly GetSavedCollectionsUseCase _getSaved;
    private readonly RegenerateSharedKeyUseCase _regenerateKey;
    private readonly UpdateCollectionUseCase _updateCollection;
    private readonly UpdateStickerUseCase _updateSticker;

    public StickerCollectionsController(
        AddCollectionUseCase addCollection,
        CreateCollectionUseCase createCollection,
        CreateStickerUseCase createSticker,
        GetAvailableCollectionsUseCase getAvailable,
        GetMyCollectionsUseCase getMy,
        GetSavedCollectionsUseCase getSaved,
        RegenerateSharedKeyUseCase regenerateKey,
        UpdateCollectionUseCase updateCollection,
        UpdateStickerUseCase updateSticker)
    {
        _addCollection = addCollection;
        _createCollection = createCollection;
        _createSticker = createSticker;
        _getAvailable = getAvailable;
        _getMy = getMy;
        _getSaved = getSaved;
        _regenerateKey = regenerateKey;
        _updateCollection = updateCollection;
        _updateSticker = updateSticker;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        var result = await _getMy.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("saved")]
    public async Task<IActionResult> GetSaved(CancellationToken ct)
    {
        var result = await _getSaved.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(CancellationToken ct)
    {
        var result = await _getAvailable.ExecuteAsync(ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCollection(CreateCollectionRequest request, CancellationToken ct)
    {
        await _createCollection.ExecuteAsync(request, ct);
        return NoContent();
    }

    [HttpPost("{sharedKey:guid}/add")]
    public async Task<IActionResult> AddCollection(Guid sharedKey, CancellationToken ct)
    {
        await _addCollection.ExecuteAsync(sharedKey, ct);
        return NoContent();
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> UpdateCollection(long id, UpdateCollectionRequest request, CancellationToken ct)
    {
        await _updateCollection.ExecuteAsync(id, request, ct);

        return NoContent();
    }

    [HttpPost("{id:long}/stickers")]
    public async Task<IActionResult> CreateSticker(long id, CreateStickerRequest request, CancellationToken ct)
    {
        await _createSticker.ExecuteAsync(id, request, ct);
        return NoContent();
    }

    [HttpPost("{id:long}/regenerate-key")]
    public async Task<IActionResult> RegenerateKey(long id, CancellationToken ct)
    {
        await _regenerateKey.ExecuteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("stickers/{publicId:guid}")]
    public async Task<IActionResult> UpdateSticker(Guid publicId, UpdateStickerRequest request, CancellationToken ct)
    {
        await _updateSticker.ExecuteAsync(publicId, request, ct);
        return NoContent();
    }
}