using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Messages;

public class MessageResponse
{
    public long Id { get; set; }
    public long ConversationId { get; set; }
    public long? SenderId { get; set; }

    public MessageType Type { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public object? Payload { get; set; }

    public IReadOnlyList<long>? MentionUserIds { get; set; }
    public MessageResponse? ReplyToMessage { get; set; }

    public bool IsRecalled { get; set; }
}