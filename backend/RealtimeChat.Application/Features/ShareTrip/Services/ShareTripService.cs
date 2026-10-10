using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Application.Features.ShareTrip.Services;

public class ShareTripService : IShareTripService
{
    private readonly IAppDbContext _context;
    private readonly IRoutingService _routingService;
    private readonly TimeProvider _timeProvider;

    private static readonly TimeSpan EtaUpdateInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan MaxEtaStaleness = TimeSpan.FromMinutes(3);
    private const double MinMovementMeters = 100;

    public ShareTripService(IAppDbContext context, IRoutingService routingService, TimeProvider timeProvider)
    {
        _context = context;
        _routingService = routingService;
        _timeProvider = timeProvider;
    }

    public async Task<UserLiveLocation> UpsertUserLocationAsync(long userId, LocationData data, CancellationToken ct)
    {
        var location = await _context.UserLiveLocations.FindAsync(userId, ct);
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        if (location is null)
        {
            location = UserLiveLocation.Create(userId, data.Coordinate, data.AccuracyMeters, data.RecordedAt, now);
            _context.UserLiveLocations.Add(location);
        }
        else
        {
            location.UpdateLocation(data.Coordinate, data.AccuracyMeters, data.RecordedAt, now);
        }

        await _context.SaveChangesAsync(ct);
        return location;
    }

    public async Task<bool> UpdateEtaIfNeededAsync(ShareTripSession trip, GeoCoordinate currentLocation, CancellationToken ct)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        if (!NeedsEtaRecalculation(trip, currentLocation, now))
        {
            return false;
        }

        var etaResult = await _routingService.GetEtaAsync(currentLocation, trip.Destination, ct);
        trip.UpdateEta(etaResult.EtaMinutes, currentLocation, now);

        await _context.SaveChangesAsync(ct);

        var hasChanged = trip.EtaMinutes != etaResult.EtaMinutes;
        
        return hasChanged;
    }

    private bool NeedsEtaRecalculation(ShareTripSession trip, GeoCoordinate currentLocation, DateTime now)
    {
        // Nếu không còn active thì không cần
        if (!trip.IsActive(now))
            return false;

        // Nếu chưa tính thì tính lại
        if (trip.HasEta())
            return true;

        var elapsed = now - trip.EtaCalculatedAt!.Value;

        // Quá 3 phút thì bắt buộc tính lại.
        if (elapsed >= MaxEtaStaleness)
            return true;

        // Chưa đủ 1 phút thì chưa tính lại.
        if (elapsed < EtaUpdateInterval)
            return false;

        // Đủ 1 phút, kiểm tra vị trí đã dịch chuyển đủ xa chưa.
        return trip.LastEtaLocation!.DistanceTo(currentLocation) >= MinMovementMeters;
    }
}