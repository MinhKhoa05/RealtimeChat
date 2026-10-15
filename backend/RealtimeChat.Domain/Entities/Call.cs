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

    public void Accept(long actorId, DateTime now)
    {
        if (actorId != ReceiverId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status != CallStatus.Ringing)
        {
            throw new DomainException("Call cannot be accepted.");
        }

        Status = CallStatus.Accepted;
        StartedAt = now;
    }

    public void Reject(long actorId)
    {
        if (actorId != ReceiverId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status != CallStatus.Ringing)
        {
            throw new DomainException("Call cannot be rejected.");
        }

        Status = CallStatus.Rejected;
    }

    public void Cancel(long actorId)
    {
        if (actorId != CallerId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status != CallStatus.Ringing)
        {
            throw new DomainException("Call cannot be cancelled.");
        }

        Status = CallStatus.Cancelled;
    }

    public void MarkAsMissed()
    {
        if (Status != CallStatus.Ringing)
        {
            throw new DomainException("Call cannot be missed.");
        }

        Status = CallStatus.Missed;
    }

    public void End(long actorId, DateTime now)
    {
        if (actorId != CallerId && actorId != ReceiverId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status != CallStatus.Accepted)
        {
            throw new DomainException("Call cannot be end.");
        }

        Status = CallStatus.Ended;
        EndedAt = now;
    }

    public TimeSpan? Duration()
    {
        if (Status != CallStatus.Ended)
        {
            throw new DomainException();
        }

        return EndedAt!.Value - StartedAt!.Value;
    }
}