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

        var trip = await _context.ShareTripSessions
            .FindAsync(shareTripId, ct)
            ?? throw new NotFoundException();

        if (trip.OwnerId != currentUserId)
        {
            throw new ForbiddenException();
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        if (!trip.IsActive(now))
        {
            throw new BadRequestException("Session is not active");
        }

        trip.End(now);
        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyToConversationAsync(trip.ConversationId, "share_trip.end", shareTripId, ct);
    }
}