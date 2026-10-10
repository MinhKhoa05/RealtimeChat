using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class Call : BaseEntity
{
    public long CallerId { get; private set; }
    public User Caller { get; private set; } = null!;

    public long ReceiverId { get; private set; }
    public User Receiver { get; private set; } = null!;

    public CallType Type { get; private set; }
    public CallStatus Status { get; private set; }

    public DateTime? StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    public TimeSpan? Duration =>
       StartedAt.HasValue && EndedAt.HasValue
           ? EndedAt.Value - StartedAt.Value
           : null;

    private Call() { }

    public static Call Create(long callerId, long receiverId, CallType type)
    {
        return new Call
        {
            CallerId = callerId,
            ReceiverId = receiverId,
            Type = type,
            Status = CallStatus.Ringing,
        };
    }

    public void Accept(DateTime now)
    {
        if (Status != CallStatus.Ringing)
        {
            throw new DomainException("Call cannot be accepted.");
        }

        Status = CallStatus.Accepted;
        StartedAt = now;
    }

    public void Reject()
    {
        if (Status != CallStatus.Ringing)
        {
            throw new DomainException("Call cannot be rejected.");
        }

        Status = CallStatus.Rejected;
    }

    public void Missed()
    {
        if (Status != CallStatus.Ringing)
        {
            throw new DomainException("Call cannot be missed.");
        }

        Status = CallStatus.Missed;
    }

    public void End(DateTime now)
    {
        if (Status != CallStatus.Accepted)
        {
            throw new DomainException("Call cannot be end.");
        }

        Status = CallStatus.Ended;
        EndedAt = now;
    }
}