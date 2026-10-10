namespace RealtimeChat.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(long userId);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}