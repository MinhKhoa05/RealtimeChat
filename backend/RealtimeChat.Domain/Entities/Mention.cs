using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Mention : BaseEntity
{
    public long MessageId { get; private set; }
    public Message Message { get; private set; } = null!;

    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int Start { get; private set; }
    public int Length { get; private set; }

    private Mention() { }

    // Factory nội bộ: Mention chỉ được tạo thông qua Message.
    internal static Mention Create(long userId, int start, int length)
    {
        if (start < 0)
        {
            throw new DomainException("Start cannot be negative.");
        }

        if (length <= 0)
        {
            throw new DomainException("Length must be greater than 0.");
        }

        return new Mention
        {
            UserId = userId,
            Start = start,
            Length = length,
        };
    }
}