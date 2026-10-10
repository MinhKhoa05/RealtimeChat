using RealtimeChat.Domain.Exceptions;
using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Domain.Entities;

public class UserLiveLocation
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public GeoCoordinate Coordinate { get; private set; } = null!;

    public float? AccuracyMeters { get; private set; }

    public DateTime RecordedAt { get; private set; } // FE ghi nhận (FE gửi lên)
    public DateTime ReceivedAt { get; private set; } // BE ghi nhận

    private UserLiveLocation() { }

    public static UserLiveLocation Create(long userId, GeoCoordinate coordinate, float? accuracyMeters, DateTime recordedAt, DateTime now)
    {
        if (userId <= 0)
        {
            throw new DomainException("User ID must be positive.");
        }

        if (accuracyMeters is < 0)
        {
            throw new DomainException("Accuracy must be non-negative.");
        }

        return new UserLiveLocation
        {
            UserId = userId,
            Coordinate = coordinate,
            AccuracyMeters = accuracyMeters,
            RecordedAt = recordedAt,
            ReceivedAt = now,
        };
    }

    public void UpdateLocation(GeoCoordinate coordinate, float? accuracyMeters, DateTime recordedAt, DateTime now)
    {
        if (accuracyMeters is < 0)
        {
            throw new DomainException("Accuracy must be non-negative.");
        }

        Coordinate = coordinate;
        AccuracyMeters = accuracyMeters;
        RecordedAt = recordedAt;
        ReceivedAt = now;
    }
}