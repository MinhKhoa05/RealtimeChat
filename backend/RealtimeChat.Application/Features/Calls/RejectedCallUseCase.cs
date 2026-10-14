using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Enums;


namespace RealtimeChat.Application.Features.Calls;

public class RejectedCallUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public RejectedCallUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long callId, CancellationToken ct)
    {
        var call = await _context.Calls
            .FirstOrDefaultAsync(x => x.Id == callId && x.Status == CallStatus.Ringing, ct)
            ?? throw new NotFoundException();

        if (call.ReceiverId != _currentUser.UserId)
        {
            throw new ForbiddenException();
        }

        call.Reject();
        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyAsync(call.CallerId, "call.rejected", call, ct);
    }
}