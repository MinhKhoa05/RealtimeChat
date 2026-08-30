namespace RealtimeChat.BLL.Contracts;

public class UserResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
}

public class TokenPayload
{
    public long UserId { get; set; }
}