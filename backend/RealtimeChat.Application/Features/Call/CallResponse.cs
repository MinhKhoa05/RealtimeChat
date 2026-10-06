using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Call;

public class CallResponse
{
    public long Id { get; set; }
    public CallType Type { get; set; }
    public CallStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public TimeSpan? Duration { get; set; }
}