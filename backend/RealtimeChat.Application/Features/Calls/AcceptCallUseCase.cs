using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Calls;

public class AcceptCallUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly TimeProvider _timeProvider;

    public AcceptCallUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _timeProvider = timeProvider;
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

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        call.Accept(now);
        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyAsync(call.CallerId, "call.acepted", call, ct);
    }
}