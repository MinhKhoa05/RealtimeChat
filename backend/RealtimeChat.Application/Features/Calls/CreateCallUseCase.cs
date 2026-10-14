using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Calls;

public class CreateCallUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public CreateCallUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task<long> ExecuteAsync(long receiverId, CancellationToken ct)
    {
        var callerId = _currentUser.UserId;

        // Chỉ bạn bè mới được gọi nhau
        var isFriend = await _context.Relationships
            .Friends()
            .Between(callerId, receiverId)
            .AnyAsync(ct);

        if (!isFriend)
        {
            throw new BadRequestException("");
        }

        var activeStatuses = new[]
        {
            CallStatus.Ringing,
            CallStatus.Accepted,
        };

        var callerIsBusy = await _context.Calls
            .AnyAsync(c =>
                (c.CallerId == callerId || c.ReceiverId == callerId) &&
                activeStatuses.Contains(c.Status),
                ct);

        if (callerIsBusy)
        {
            throw new ConflictException("You are already in an active call.");
        }

        var receiverIsBusy = await _context.Calls
            .AnyAsync(c =>
                (c.CallerId == receiverId || c.ReceiverId == receiverId) &&
                activeStatuses.Contains(c.Status),
                ct);

        if (receiverIsBusy)
        {
            throw new ConflictException("The recipient is already in an active call.");
        }

        var call = Call.Create(callerId, receiverId, Domain.Enums.CallType.Audio);
        _context.Calls.Add(call);

        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyAsync(receiverId, "call.ringing", call, ct);
        return call.Id;
    }
}