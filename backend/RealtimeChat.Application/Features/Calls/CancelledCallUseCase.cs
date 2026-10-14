using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.Features.Calls.Services;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Calls;

public class CancelledCallUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly ICallHistoryService _callHistory;
    private readonly IUnitOfWork _unitOfWork;

    public CancelledCallUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, ICallHistoryService callHistory, IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _callHistory = callHistory;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(long callId, CancellationToken ct)
    {
        var result = await _unitOfWork.ExecuteInTransactionAsync(
            async token =>
            {
                var call = await _context.Calls.FirstOrDefaultAsync(x => x.Id == callId && x.Status == CallStatus.Ringing, token)
                    ?? throw new NotFoundException();

                call.Cancelled(_currentUser.UserId);

                var message = await _callHistory.CreateAsync(call, token);

                // Lưu trạng thái Call và Message.
                await _context.SaveChangesAsync(token);

                return (Call: call, Message: message);
            },
            ct);

        // Chỉ thông báo sau khi transaction commit thành công.
        await _notifier.NotifyAsync(result.Call.ReceiverId, "call.cancelled", result.Call, ct);

        await _notifier.NotifyAsync(result.Call.ReceiverId, "message.created", result.Message, ct);
    }
}