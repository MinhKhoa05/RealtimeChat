using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Domain.Entities;

public class Call
{
    public long Id { get; set; }

    public long CallerId { get; set; }
    public User Caller { get; set; } = null!;

    public long ReceiverId {get; set; }
    public User Receiver {get; set; } = null!;

    public CallType Type { get; set; }
    public CallStatus Status { get; set; }

    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}