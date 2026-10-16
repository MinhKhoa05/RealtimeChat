namespace RealtimeChat.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt {get; protected set; }
}