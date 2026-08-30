using RealtimeChat.DAL.Entities;

namespace RealtimeChat.BLL.Context;

public class UserContext
{
    public long UserId { get; init; }
    public string Email { get; init; } = string.Empty;
}
