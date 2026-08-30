using System.Security.Claims;
using RealtimeChat.BLL.Context;
using Microsoft.AspNetCore.Mvc;

namespace RealtimeChat.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ApiOk() => Ok(ApiResponse.Ok());

    protected IActionResult ApiOk<T>(T? data) => Ok(ApiResponse.Ok(data));

    protected IActionResult ApiCreated<T>(T? data)
        => StatusCode(StatusCodes.Status201Created, ApiResponse.Ok(data));

    protected IActionResult ApiCreated()
        => StatusCode(StatusCodes.Status201Created, ApiResponse.Ok());

    protected UserContext CurrentUser
    {
        get
        {
            return new UserContext
            {
                UserId = long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : 0,
            };
        }
    }
}
