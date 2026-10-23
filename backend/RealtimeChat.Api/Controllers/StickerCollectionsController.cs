using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Stickers;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sticker-collections")]
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

    /// <summary>
    /// Lấy danh sách bộ sticker do người dùng hiện tại tạo
    /// </summary>
    [HttpGet("me")]
    public async Task<ActionResult<IReadOnlyList<CollectionResponse>>> GetMy(CancellationToken ct)
    {
        var result = await _getMy.ExecuteAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách bộ sticker đã lưu của người dùng hiện tại
    /// </summary>
    [HttpGet("saved")]
    public async Task<ActionResult<IReadOnlyList<CollectionResponse>>> GetSaved(CancellationToken ct)
    {
        var result = await _getSaved.ExecuteAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách tất cả bộ sticker khả dụng
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult<IReadOnlyList<CollectionResponse>>> GetAvailable(CancellationToken ct)
    {
        var result = await _getAvailable.ExecuteAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// Tạo bộ sưu tập sticker mới
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateCollectionResponse>> CreateCollection(CreateCollectionRequest request, CancellationToken ct)
    {
        var result = await _createCollection.ExecuteAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Lưu bộ sưu tập sticker theo shared key
    /// </summary>
    [HttpPost("{sharedKey:guid}/add")]
    public async Task<IActionResult> AddCollection(Guid sharedKey, CancellationToken ct)
    {
        await _addCollection.ExecuteAsync(sharedKey, ct);
        return NoContent();
    }

    /// <summary>
    /// Cập nhật thông tin bộ sưu tập sticker
    /// </summary>
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> UpdateCollection(long id, UpdateCollectionRequest request, CancellationToken ct)
    {
        await _updateCollection.ExecuteAsync(id, request, ct);
        return NoContent();
    }

    /// <summary>
    /// Thêm sticker mới vào bộ sưu tập
    /// </summary>
    [HttpPost("{id:long}/stickers")]
    public async Task<ActionResult<StickerResponse>> CreateSticker(long id, CreateStickerRequest request, CancellationToken ct)
    {
        var result = await _createSticker.ExecuteAsync(id, request, ct);
        return Ok(result);
    }

    /// <summary>
    /// Tạo lại shared key cho bộ sưu tập sticker
    /// </summary>
    [HttpPost("{id:long}/regenerate-key")]
    public async Task<IActionResult> RegenerateKey(long id, CancellationToken ct)
    {
        await _regenerateKey.ExecuteAsync(id, ct);
        return NoContent();
    }

    /// <summary>
    /// Cập nhật thông tin sticker
    /// </summary>
    [HttpPatch("stickers/{publicId:guid}")]
    public async Task<IActionResult> UpdateSticker(Guid publicId, UpdateStickerRequest request, CancellationToken ct)
    {
        await _updateSticker.ExecuteAsync(publicId, request, ct);
        return NoContent();
    }
}