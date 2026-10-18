namespace RealtimeChat.Domain.Entities;

public class Mention
{
    public long MessageId { get; private set; }
    public Message Message { get; private set; } = null!;
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    private Mention() { }

    // Chỉ được gọi thông qua Message
    internal static Mention Create(long userId)
    {
        return new Mention
        {
            UserId = userId,
        };
    }
}