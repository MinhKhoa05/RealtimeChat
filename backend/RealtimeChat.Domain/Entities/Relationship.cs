using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class Relationship
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long TargetUserId { get; set; }
    public User TargetUser { get; set; } = null!;

    public RelationshipType Type { get; set; }

    public string? Introduction { get; set; }
    public DateTime CreatedAt { get; set; }
}