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
    private readonly IShareTripService _shareTripService;

    public CreateShareTripUseCase(IAppDbContext context, ICurrentUser currentUser, IClientNotifier notifier, IShareTripService shareTripService)
    {
        _context = context;
        _currentUser = currentUser;
        _notifier = notifier;
        _shareTripService = shareTripService;
    }

    public async Task ExecuteAsync(long conversationId, CreateShareTripRequest request, CancellationToken ct)
    {
        var actorId = _currentUser.UserId;

        var isMember = await _context.Conversations.IsMemberAsync(conversationId, actorId, ct);
        if (!isMember)
        {
            throw new ForbiddenException();
        }

        var exsits = await _context.ShareTripSessions
            .Where(x => x.Status == Domain.Enums.ShareTripStatus.Active)
            .Where(x => x.OwnerId == actorId && x.ConversationId == conversationId)
            .AnyAsync(ct);

        if (!exsits)
        {
            throw new ConflictException();
        }

        var trip = ShareTripSession.Create(actorId, conversationId, request.TripTitle, request.Destination, DateTime.UtcNow);
        _context.ShareTripSessions.Add(trip);

        await _context.SaveChangesAsync(ct);

        var userLiveLocation = await _shareTripService.UpsertUserLocationAsync(actorId, request.CurrentLocationData, ct);

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