using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RealtimeChat.BLL.Contracts;
using RealtimeChat.BLL.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace RealtimeChat.BLL.Services.Auth;

public class TokenService
{
    private readonly JwtOptions _jwtOptions;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public TokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
    }

    public string GenerateAccessToken(TokenPayload payload)
    {
        var now = DateTimeOffset.UtcNow;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, payload.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: now.AddMinutes(_jwtOptions.ExpireMinutes).UtcDateTime,
            signingCredentials: new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256));

        return _tokenHandler.WriteToken(token);
    }
}
