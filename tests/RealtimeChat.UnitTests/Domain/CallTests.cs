using FluentAssertions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.UnitTests.Domain;

public class CallTests
{
    private const long CallerIdDefault = 1;
    private const long ReceiverIdDefault = 2;
    private const long OtherUserId = 3;

    private static Call CreateCallDefault()
    {
        return Call.Create(CallerIdDefault, ReceiverIdDefault, CallType.Audio);
    }

    private static Call CreateCallDefaultWithStatus(CallStatus status)
    {
        var call = CreateCallDefault();

        switch (status)
        {
            case CallStatus.Ringing:
                break;

            case CallStatus.Accepted:
                call.Accept(ReceiverIdDefault, StartedAt);
                break;

            case CallStatus.Rejected:
                call.Reject(ReceiverIdDefault);
                break;

            case CallStatus.Cancelled:
                call.Cancel(CallerIdDefault);
                break;

            case CallStatus.Missed:
                call.MarkAsMissed();
                break;

            case CallStatus.Ended:
                call.Accept(ReceiverIdDefault, StartedAt);
                call.End(ReceiverIdDefault, StartedAt.AddMinutes(1));
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return call;
    }

    private static readonly DateTime StartedAt = new(2026, 10, 15, 10, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(CallType.Audio)]
    [InlineData(CallType.Video)]
    public void Create_WithValidInput_CreatesRingingCall(CallType type)
    {
        var call = Call.Create(CallerIdDefault, ReceiverIdDefault, type);

        call.CallerId.Should().Be(CallerIdDefault);
        call.ReceiverId.Should().Be(ReceiverIdDefault);
        call.Type.Should().Be(type);
        call.Status.Should().Be(CallStatus.Ringing);
        call.StartedAt.Should().BeNull();
        call.EndedAt.Should().BeNull();
    }

    [Fact]
    public void Accept_WhenActorIsReceiver_StartsCall()
    {
        var call = CreateCallDefault();
        var actorId = ReceiverIdDefault;

        call.Accept(actorId, StartedAt);

        call.Status.Should().Be(CallStatus.Accepted);
        call.StartedAt.Should().Be(StartedAt);
    }

    [Fact]
    public void Accept_WhenActorIsNotReceiver_ThrowsForbiddenDomainException()
    {
        var call = CreateCallDefault();
        var actorId = OtherUserId;

        var act = () => call.Accept(actorId, StartedAt);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Theory]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void Accept_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateCallDefaultWithStatus(status);
        var actorId = ReceiverIdDefault;

        var act = () => call.Accept(actorId, StartedAt);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Reject_WhenActorIsReceiver_RejectsCall()
    {
        var call = CreateCallDefault();
        var actorId = ReceiverIdDefault;

        call.Reject(actorId);

        call.Status.Should().Be(CallStatus.Rejected);
    }

    [Fact]
    public void Reject_WhenActorIsNotReceiver_ThrowsForbiddenDomainException()
    {
        var call = CreateCallDefault();
        var actorId = OtherUserId;

        var act = () => call.Reject(actorId);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Theory]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void Reject_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateCallDefaultWithStatus(status);
        var actorId = ReceiverIdDefault;

        var act = () => call.Reject(actorId);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Cancel_WhenActorIsCaller_CancelsCall()
    {
        var call = CreateCallDefault();
        var actorId = CallerIdDefault;

        call.Cancel(actorId);

        call.Status.Should().Be(CallStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenActorIsNotCaller_ThrowsForbiddenDomainException()
    {
        var call = CreateCallDefault();
        var actorId = OtherUserId;

        var act = () => call.Cancel(actorId);

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Theory]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void Cancel_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateCallDefaultWithStatus(status);
        var actorId = CallerIdDefault;

        var act = () => call.Cancel(actorId);

        act.Should().Throw<DomainException>();
    }


    [Fact]
    public void MarkAsMissed_WhenCallIsRinging_MarksCallAsMissed()
    {
        var call = CreateCallDefault();

        call.MarkAsMissed();

        call.Status.Should().Be(CallStatus.Missed);
    }

    [Theory]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void MarkAsMissed_WhenCallIsNotRinging_ThrowsDomainException(CallStatus status)
    {
        var call = CreateCallDefaultWithStatus(status);

        var act = () => call.MarkAsMissed();

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(CallerIdDefault)]
    [InlineData(ReceiverIdDefault)]
    public void End_WhenActorIsParticipant_EndsAcceptedCall(long actorId)
    {
        var call = CreateCallDefaultWithStatus(CallStatus.Accepted);

        var endTime = StartedAt.AddMinutes(1);

        call.End(actorId, endTime);

        call.Status.Should().Be(CallStatus.Ended);
        call.EndedAt.Should().Be(endTime);
    }

    [Fact]
    public void End_WhenActorIsNotParticipant_ThrowsForbiddenDomainException()
    {
        var call = CreateCallDefaultWithStatus(CallStatus.Accepted);
        var actorId = OtherUserId;

        var act = () => call.End(actorId, StartedAt.AddMinutes(1));

        act.Should().Throw<ForbiddenDomainException>();
    }

    [Theory]
    [InlineData(CallStatus.Ringing)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    [InlineData(CallStatus.Ended)]
    public void End_WhenCallIsNotAccepted_ThrowsDomainException(CallStatus status)
    {
        var call = CreateCallDefaultWithStatus(status);
        var actorId = CallerIdDefault;

        var act = () => call.End(actorId, StartedAt.AddMinutes(1));

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Duration_WhenCallIsEnded_ReturnsDuration()
    {
        var call = CreateCallDefaultWithStatus(CallStatus.Ended);

        call.Duration().Should().Be(TimeSpan.FromMinutes(1));
    }

    [Theory]
    [InlineData(CallStatus.Ringing)]
    [InlineData(CallStatus.Accepted)]
    [InlineData(CallStatus.Rejected)]
    [InlineData(CallStatus.Cancelled)]
    [InlineData(CallStatus.Missed)]
    public void Duration_WhenCallIsNotEnded_ThrowsDomainException(
    CallStatus status)
    {
        var call = CreateCallDefaultWithStatus(status);

        var act = () => call.Duration();

        act.Should().Throw<DomainException>();
    }
}