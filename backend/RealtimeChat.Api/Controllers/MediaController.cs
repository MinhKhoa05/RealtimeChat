using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Features.Media;

namespace RealtimeChat.Api.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/media")]
public class MediaController : ControllerBase
{
    private readonly UploadMediaUseCase _upload;

    public MediaController(UploadMediaUseCase upload)
    {
        _upload = upload;
    }

    /// <summary>
    /// Tải lên tệp media (ảnh, video, tệp đính kèm)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MediaResponse>> Upload(IFormFile file, CancellationToken ct)
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
}
