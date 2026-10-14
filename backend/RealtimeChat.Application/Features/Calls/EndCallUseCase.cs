using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Enums;

namespace RealtimeChat.Application.Features.Calls;

public class EndCallUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly TimeProvider _timeProvider;

    public EndCallUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(long callId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var call = await _context.Calls
            .FirstOrDefaultAsync(x => x.Id == callId, ct)
            ?? throw new NotFoundException();

        // Chỉ người gọi hoặc người nhận mới được kết thúc
        if (call.CallerId != currentUserId &&
            call.ReceiverId != currentUserId)
        {
            throw new ForbiddenException();
        }

        if (call.Status != CallStatus.Accepted)
        {
            throw new BadRequestException("This call cannot be ended in its current state.");
        }

        var otherUserId = call.CallerId == currentUserId
            ? call.ReceiverId
            : call.CallerId;

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        call.End(now);

        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyAsync(otherUserId, "call.end", call, ct);
    }
}
