using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Application.Features.ShareTrip.Services;

public interface IShareTripService
{
    Task<UserLiveLocation> UpsertUserLocationAsync(long userId, LocationData data, CancellationToken cancellationToken);
    Task<bool> UpdateEtaIfNeededAsync(ShareTripSession trip, GeoCoordinate currentLocation, CancellationToken cancellationToken);
}

public class LocationData
{
    public GeoCoordinate Coordinate { get; set; } = null!;
    public float? AccuracyMeters { get; set; }
    public DateTime RecordedAt { get; set; }
}