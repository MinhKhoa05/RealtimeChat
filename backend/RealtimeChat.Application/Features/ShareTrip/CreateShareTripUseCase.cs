using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Features.ShareTrip;

public class CreateShareTripUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly TimeProvider _timeProvider;


    public CreateShareTripUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _timeProvider = timeProvider;
    }

    public async Task ExecuteAsync(long conversationId, CreateShareTripRequest request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var canAccessConversation = await _context.Conversations
            .FilterAccessible(conversationId, currentUserId)
            .AnyAsync(ct);

        if (!canAccessConversation)
        {
            throw new ForbiddenException();
        }

        var exsits = await _context.ShareTripSessions
            .Where(x => x.Status == Domain.Enums.ShareTripStatus.Active)
            .Where(x => x.OwnerId == currentUserId && x.ConversationId == conversationId)
            .AnyAsync(ct);

        if (!exsits)
        {
            throw new ConflictException();
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var shareTripSession = ShareTripSession.Create(currentUserId, conversationId, request.TripTitle,
            request.DestinationLatitude, request.DestinationLongitude, now);

        _context.ShareTripSessions.Add(shareTripSession);

        // Tạo mới hoặc cập nhật vị trí mới nhất
        var userLiveLocation = await _context.UserLiveLocations
            .FindAsync(currentUserId, ct);

        if (userLiveLocation is null)
        {
            userLiveLocation = UserLiveLocation.Create(currentUserId, request.CurrentLatitude,
                request.CurrentLongitude, request.AccuracyMeters, request.RecordedAt, now);

            _context.UserLiveLocations.Add(userLiveLocation);
        }
        else
        {
            userLiveLocation.UpdateLocation(request.CurrentLatitude, request.CurrentLongitude, request.AccuracyMeters, request.RecordedAt, now);
        }

        await _context.SaveChangesAsync(ct);
        await _notifier.NotifyToConversationAsync(conversationId, "share_trip", shareTripSession, ct);

        // Gọi Routing API để tính ETA.
        // session.UpdateEta(etaMinutes, latitude, longitude, now);
        // await _context.SaveChangesAsync(ct);
        // await _notifier.NotifyToConversationAsync(conversationId, "share_trip_eta_updated", response, ct);
    }
}

public class CreateShareTripRequest
{
    public string TripTitle { get; set; } = string.Empty;
    public decimal DestinationLatitude { get; set; }
    public decimal DestinationLongitude { get; set; }

    public decimal CurrentLatitude { get; set; }
    public decimal CurrentLongitude { get; set; }
    public DateTime RecordedAt { get; set; }

    public float? AccuracyMeters { get; set; }
}
