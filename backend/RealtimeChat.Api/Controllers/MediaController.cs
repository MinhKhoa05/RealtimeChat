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

    public MediaController(UploadMediaUseCase upload)
    {
        _upload = upload;
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
}
