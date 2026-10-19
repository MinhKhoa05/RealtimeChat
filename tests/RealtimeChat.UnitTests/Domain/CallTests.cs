using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class CallTests
{
    private const long DefaultCallerId = 1L;
    private const long DefaultReceiverId = 2L;
    private const long DefaultUnauthorizedUserId = 3L;

    private static readonly DateTime DefaultStartedAt =
        new(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);

    private static Call CreateDefault(CallStatus status = CallStatus.Ringing)
    {
        var call = Call.Create(DefaultCallerId, DefaultReceiverId, CallType.Audio);

        switch (status)
        {
            case CallStatus.Ringing:
                break;

            case CallStatus.Accepted:
                call.Accept(DefaultReceiverId, DefaultStartedAt);
                break;

            case CallStatus.Rejected:
                call.Reject(DefaultReceiverId);
                break;

            case CallStatus.Cancelled:
                call.Cancel(DefaultCallerId);
                break;

            case CallStatus.Missed:
                call.MarkAsMissed();
                break;

            case CallStatus.Ended:
                call.Accept(DefaultReceiverId, DefaultStartedAt);
                call.End(DefaultReceiverId, DefaultStartedAt.AddMinutes(1));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }

        return call;
    }

    [Theory]
    [InlineData(CallType.Audio)]
    [InlineData(CallType.Video)]
    public void Create_WithValidInput_CreatesRingingCall(CallType type)
    {
        var call = Call.Create(DefaultCallerId, DefaultReceiverId, type);

        call.CallerId.Should().Be(DefaultCallerId);
        call.ReceiverId.Should().Be(DefaultReceiverId);
        call.Type.Should().Be(type);
        call.Status.Should().Be(CallStatus.Ringing);
        call.StartedAt.Should().BeNull();
        call.EndedAt.Should().BeNull();
    }

    [Fact]
    public void Create_WhenCallerAndReceiverEquals_ThrowsDomainException()
    {
        var act = () => Call.Create(DefaultCallerId, DefaultCallerId, CallType.Audio);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Accept_WhenActorIsReceiver_StartsCall()
    {
        var call = CreateDefault();
        var actorId = DefaultReceiverId;

        var isChange = call.Accept(actorId, DefaultStartedAt);

        call.Status.Should().Be(CallStatus.Accepted);
        call.StartedAt.Should().Be(DefaultStartedAt);
        isChange.Should().BeTrue();
    }

    [Theory]
    [InlineData(DefaultUnauthorizedUserId)]
    [InlineData(DefaultCallerId)]
    public void Accept_WhenActorIsNotReceiver_ThrowsForbiddenDomainException(long actorId)
    {
        var call = CreateDefault();

        var act = () => call.Accept(actorId, DefaultStartedAt);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void Accept_WhenCallAlreadyAccepted_ReturnFalse()
    {
        var call = CreateDefault(CallStatus.Accepted);
        var actorId = DefaultReceiverId;

        var isChange = call.Accept(actorId, DefaultStartedAt);

        isChange.Should().BeFalse();
    }

    [Theory]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void Accept_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateDefault(status);
        var actorId = DefaultReceiverId;

        var act = () => call.Accept(actorId, DefaultStartedAt);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Reject_WhenActorIsReceiver_RejectsCall()
    {
        var call = CreateDefault();
        var actorId = DefaultReceiverId;

        var isChange = call.Reject(actorId);

        call.Status.Should().Be(CallStatus.Rejected);
        isChange.Should().BeTrue();
    }

    [Theory]
    [InlineData(DefaultUnauthorizedUserId)]
    [InlineData(DefaultCallerId)]
    public void Reject_WhenActorIsNotReceiver_ThrowsForbiddenDomainException(long actorId)
    {
        var call = CreateDefault();

        var act = () => call.Reject(actorId);

        act.Should().Throw<ForbiddenDomainException>();
    }

     [Fact]
    public void Rejected_WhenCallAlreadyRejected_ReturnFalse()
    {
        var call = CreateDefault(CallStatus.Rejected);

        var isChange = call.Reject(DefaultReceiverId);

        isChange.Should().BeFalse();
    }

    [Theory]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void Reject_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateDefault(status);
        var actorId = DefaultReceiverId;

        var act = () => call.Reject(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_WhenActorIsCaller_CancelsCall()
    {
        var call = CreateDefault();
        var actorId = DefaultCallerId;

        var isChange = call.Cancel(actorId);

        call.Status.Should().Be(CallStatus.Cancelled);
        isChange.Should().BeTrue();
    }

    [Theory]
    [InlineData(DefaultReceiverId)]
    [InlineData(DefaultUnauthorizedUserId)]
    public void Cancel_WhenActorIsNotCaller_ThrowsForbiddenDomainException(long actorId)
    {
        var call = CreateDefault();

        var act = () => call.Cancel(actorId);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void Cancel_WhenCallIsAlreadyCancelled_ReturnFalse()
    {
        var call = CreateDefault(CallStatus.Cancelled);

        var isChange = call.Cancel(DefaultCallerId);
        
        isChange.Should().BeFalse();
    }

    [Theory]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void Cancel_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateDefault(status);
        var actorId = DefaultCallerId;

        var act = () => call.Cancel(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MarkAsMissed_WhenCallIsRinging_MarksCallAsMissed()
    {
        var call = CreateDefault();

        var isChange = call.MarkAsMissed();

        call.Status.Should().Be(CallStatus.Missed);
        isChange.Should().BeTrue();
    }

    [Fact]
    public void MarkAsMissed_WhenCallIsAlreadyMissed_ReturnFalse()
    {
        var call = CreateDefault(CallStatus.Missed);

        var isChange = call.MarkAsMissed();

        isChange.Should().BeFalse();
    }

    [Theory]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Ended)]
    public void MarkAsMissed_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateDefault(status);

        var act = () => call.MarkAsMissed();

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(DefaultCallerId)]
    [InlineData(DefaultReceiverId)]
    public void End_WhenActorIsParticipant_EndsAcceptedCall(long actorId)
    {
        var call = CreateDefault(CallStatus.Accepted);
        var endTime = DefaultStartedAt.AddMinutes(1);

        var isChange = call.End(actorId, endTime);

        call.Status.Should().Be(CallStatus.Ended);
        call.EndedAt.Should().Be(endTime);
        isChange.Should().BeTrue();
    }

    [Fact]
    public void End_WhenActorIsNotParticipant_ThrowsForbiddenDomainException()
    {
        var call = CreateDefault(CallStatus.Accepted);
        var actorId = DefaultUnauthorizedUserId;

        var act = () => call.End(actorId, DefaultStartedAt.AddMinutes(1));

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Fact]
    public void End_WhenCallIsAlreadyEnded_ReturnFalse()
    {
        var call = CreateDefault(CallStatus.Ended);

        var isChange = call.End(DefaultCallerId, DefaultStartedAt.AddMinutes(1));

        isChange.Should().BeFalse();
    }

    [Theory]
    [InlineData(CallStatus.Ringing)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    public void End_WhenCallIsNotAccepted_ThrowsDomainException(CallStatus status)
    {
        var call = CreateDefault(status);
        var actorId = DefaultCallerId;

        var act = () => call.End(actorId, DefaultStartedAt.AddMinutes(1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Duration_WhenCallIsEnded_ReturnDuration()
    {
        var call = CreateDefault(CallStatus.Accepted);
        var duration = TimeSpan.FromMinutes(1);
        call.End(DefaultCallerId, DefaultStartedAt.Add(duration));

        var result = call.Duration;

        result.Should().Be(duration);
    }

    [Theory]
    [InlineData(CallStatus.Ringing)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    public void Duration_WhenCallIsNotEnded_ReturnNull(CallStatus status)
    {
        var call = CreateDefault(status);

        var result = call.Duration;

        result.Should().BeNull();
    }
}
