using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class Conversation
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public ConversationType Type { get; set; }

    public long? AvatarMediaId {get; set; }
    public Media? AvatarMedia {get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime DisbandedAt {get; set; }

    public ICollection<ConversationMember> Members { get; set; }
        = new List<ConversationMember>();

    public ICollection<Message> Messages { get; set; }
        = new List<Message>();
}