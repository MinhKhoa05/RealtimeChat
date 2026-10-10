using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.ShareTrip;

public class EndShareTripUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly TimeProvider _timeProvider;

    public EndShareTripUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(long shareTripId, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var shareTripSession = await _context.ShareTripSessions
            .FindAsync(shareTripId, ct)
            ?? throw new NotFoundException();

        if (shareTripSession.OwnerId != currentUserId)
        {
            throw new ForbiddenException();
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        if (!shareTripSession.IsActive(now))
        {
            throw new BadRequestException("Session is not active");
        }

        shareTripSession.End(now);
        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyToConversationAsync(shareTripSession.ConversationId, "share_trip.end", shareTripId, ct);
    }
}