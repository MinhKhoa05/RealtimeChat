namespace RealtimeChat.Api.Extensions;

public static class CookieExtensions
{
    private const string AccessTokenCookieName = "accessToken";

    public static void SetAccessToken(this HttpResponse response, string accessToken, int expireMinutes, bool secure)
    {
        if (response.HasStarted || string.IsNullOrEmpty(accessToken))
        {
            return;
        }

        response.Cookies.Append(
            AccessTokenCookieName,
            accessToken,
            CreateAuthCookieOptions(secure, "/", DateTimeOffset.UtcNow.AddMinutes(expireMinutes)));
    }

    public static void DeleteAccessToken(this HttpResponse response, bool secure)
    {
        if (response.HasStarted)
        {
            return;
        }

        response.Cookies.Delete(AccessTokenCookieName, CreateAuthCookieOptions(secure, "/"));
    }

    private static CookieOptions CreateAuthCookieOptions(bool secure, string path, DateTimeOffset? expires = null)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = SameSiteMode.Lax,
            Expires = expires,
            Path = path
        };
    }
}
