using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Features.Calls.Services;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Calls;

public class EndCallUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly TimeProvider _timeProvider;
    private readonly ICallHistoryService _callHistory;
    private readonly IUnitOfWork _unitOfWork;

    public EndCallUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, ICallHistoryService callHistory, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _callHistory = callHistory;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(long callId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var result = await _unitOfWork.ExecuteInTransactionAsync(
            async token =>
            {
                var call = await _context.Calls
                    .FirstOrDefaultAsync(x => x.Id == callId, token)
                    ?? throw new NotFoundException();

                var otherUserId = call.CallerId == actorId
                    ? call.ReceiverId
                    : call.CallerId;

                call.End(actorId, _timeProvider.GetUtcNow().UtcDateTime);

                var message = await _callHistory.CreateAsync(call, token);

                await _context.SaveChangesAsync(token);

                return (Call: call, Message: message, OtherUserId: otherUserId);
            },
            ct);

        // Chỉ thông báo sau khi transaction commit thành công.
        await _notifier.NotifyAsync(result.OtherUserId, "call.end", result.Call, ct);

        await _notifier.NotifyAsync(result.OtherUserId, "message.created", result.Message, ct);
    }
}
