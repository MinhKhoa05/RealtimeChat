using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Features.ShareTrip.Services;
using RealtimeChat.Application.Interfaces;

namespace RealtimeChat.Application.Features.ShareTrip;

public class UpdateUserLiveLocationUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly IShareTripService _shareTripService;

    public UpdateUserLiveLocationUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, IShareTripService shareTripService)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _shareTripService = shareTripService;
    }

    public async Task ExecuteAsync(LocationData data, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var userLiveLocation = await _shareTripService.UpsertUserLocationAsync(currentUserId, data, ct);

        // Side effect: cập nhật ETA các chuyến đi đang chia sẻ và thông báo nếu ETA thay đổi.
        var trips = await _context.ShareTripSessions
            .AsNoTracking()
            .Where(x => x.Status == Domain.Enums.ShareTripStatus.Active)
            .Where(x => x.OwnerId == currentUserId)
            .ToListAsync(ct);

        foreach (var trip in trips)
        {
            var hasChanged = await _shareTripService.UpdateEtaIfNeededAsync(trip, userLiveLocation.Coordinate, ct);

            if (!hasChanged) continue;

            await _notifier.NotifyToConversationAsync(trip.ConversationId, "eta.update", trip, ct);
        }
    }
}