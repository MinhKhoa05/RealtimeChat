using System.Security.Claims;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Api.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long UserId =>
        long.Parse(
            _httpContextAccessor.HttpContext!
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier)!);
}