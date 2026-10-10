using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Application.Features.ShareTrip.Services;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.QueryExtensions;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Application.Features.ShareTrip;

public class CreateShareTripUseCase
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IClientNotifier _notifier;
    private readonly TimeProvider _timeProvider;
    private readonly IShareTripService _shareTripService;

    public CreateShareTripUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, TimeProvider timeProvider, IShareTripService shareTripService)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _timeProvider = timeProvider;
        _shareTripService = shareTripService;
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

        var trip = ShareTripSession.Create(currentUserId, conversationId, request.TripTitle, request.Destination, now);
        _context.ShareTripSessions.Add(trip);

        await _context.SaveChangesAsync(ct);

        var userLiveLocation = await _shareTripService.UpsertUserLocationAsync(currentUserId, request.CurrentLocationData, ct);

        await _notifier.NotifyToConversationAsync(conversationId, "share_trip", trip, ct);

        await _shareTripService.UpdateEtaIfNeededAsync(trip, userLiveLocation.Coordinate, ct);
        await _notifier.NotifyToConversationAsync(trip.ConversationId, "eta.update", trip, ct);
    }
}

public class CreateShareTripRequest
{
    public string TripTitle { get; set; } = string.Empty;
    public GeoCoordinate Destination { get; set; } = null!;
    public LocationData CurrentLocationData { get; set; } = null!;
}