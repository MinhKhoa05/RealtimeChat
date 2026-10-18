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

    public TimeSpan? Duration
        => Status == CallStatus.Ended
            ? EndedAt!.Value - StartedAt!.Value
            : null;

    private Call() { }

    public static Call Create(long callerId, long receiverId, CallType type)
    {
        if (callerId == receiverId)
        {
            throw new DomainException("Caller and receiver must be different.");
        }

        return new Call
        {
            CallerId = callerId,
            ReceiverId = receiverId,
            Type = type,
            Status = CallStatus.Ringing
        };
    }

    public bool Accept(long actorId, DateTime now)
    {
        if (actorId != ReceiverId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status == CallStatus.Accepted)
        {
            return false;
        }

        EnsureStatus(CallStatus.Ringing, "accepted");

        Status = CallStatus.Accepted;
        StartedAt = now;
        return true;
    }

    public bool Reject(long actorId)
    {
        if (actorId != ReceiverId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status == CallStatus.Rejected)
        {
            return false;
        }

        EnsureStatus(CallStatus.Ringing, "rejected");

        Status = CallStatus.Rejected;
        return true;
    }

    public bool Cancel(long actorId)
    {
        if (actorId != CallerId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status == CallStatus.Cancelled)
        {
            return false;
        }

        EnsureStatus(CallStatus.Ringing, "cancelled");

        Status = CallStatus.Cancelled;
        return true;
    }

    public bool MarkAsMissed()
    {
        if (Status == CallStatus.Missed)
        {
            return false;
        }

        EnsureStatus(CallStatus.Ringing, "marked as missed");

        Status = CallStatus.Missed;
        return true;
    }

    public bool End(long actorId, DateTime now)
    {
        if (actorId != CallerId && actorId != ReceiverId)
        {
            throw new ForbiddenDomainException();
        }

        if (Status == CallStatus.Ended)
        {
            return false;
        }

        EnsureStatus(CallStatus.Accepted, "ended");

        Status = CallStatus.Ended;
        EndedAt = now;
        return true;
    }

    private void EnsureStatus(CallStatus expectedStatus, string action)
    {
        if (Status != expectedStatus)
        {
            throw new DomainException($"Call cannot be {action}.");
        }
    }
}
