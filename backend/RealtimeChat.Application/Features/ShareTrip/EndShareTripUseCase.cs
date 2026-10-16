using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.ShareTrip;

public class EndShareTripUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;

    public EndShareTripUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task ExecuteAsync(long shareTripId, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var trip = await _context.ShareTripSessions
            .FindAsync(shareTripId, ct)
            ?? throw new NotFoundException();

        trip.End(actorId, DateTime.UtcNow);
        await _context.SaveChangesAsync(ct);

        await _notifier.NotifyToConversationAsync(trip.ConversationId, "share_trip.end", shareTripId, ct);
    }
}