using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Media;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/media")]
public class MediaController : ControllerBase
{
    private readonly UploadMediaUseCase _upload;
    private readonly GetMediaUseCase _getMedia;

    public MediaController(
        UploadMediaUseCase upload,
        GetMediaUseCase getMedia)
    {
        _upload = upload;
        _getMedia = getMedia;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        await using var stream = file.OpenReadStream();

        var request = new UploadMediaRequest
        {
            Content = stream,
            OriginalName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length
        };

        var result = await _upload.ExecuteAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{publicId:guid}")]
    public async Task<IActionResult> GetMedia(Guid publicId, CancellationToken ct)
    {
        var result = await _getMedia.ExecuteAsync(publicId, ct);
        return File(result.Stream, result.ContentType);
    }

    [HttpGet("{publicId:guid}/download")]
    public async Task<IActionResult> Download(Guid publicId, CancellationToken ct)
    {
        var result = await _getMedia.ExecuteAsync(publicId, ct);
        return File(result.Stream, result.ContentType, result.FileName);
    }
}
