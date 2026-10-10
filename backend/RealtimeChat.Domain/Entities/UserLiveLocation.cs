using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class UserLiveLocation
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    public float? AccuracyMeters { get; private set; }

    public DateTime RecordedAt { get; private set; } // FE ghi nhận (FE gửi lên)
    public DateTime ReceivedAt { get; private set; } // BE ghi nhận

    private UserLiveLocation() { }

    public static UserLiveLocation Create(long userId, decimal latitude, decimal longitude, float? accuracyMeters, DateTime recordedAt)
    {
        if (userId <= 0)
            throw new DomainException("User ID must be positive.");

        if (latitude is < -90 or > 90)
            throw new DomainException("Latitude must be between -90 and 90.");

        if (longitude is < -180 or > 180)
            throw new DomainException("Longitude must be between -180 and 180.");

        if (accuracyMeters is < 0)
            throw new DomainException("Accuracy must be non-negative.");

        return new UserLiveLocation
        {
            UserId = userId,
            Latitude = latitude,
            Longitude = longitude,
            AccuracyMeters = accuracyMeters,
            RecordedAt = recordedAt,
            ReceivedAt = DateTime.UtcNow,
        };
    }

    public void UpdateLocation(decimal latitude, decimal longitude, float? accuracyMeters, DateTime recordedAt)
    {
        if (latitude is < -90 or > 90)
            throw new DomainException("Latitude must be between -90 and 90.");

        if (longitude is < -180 or > 180)
            throw new DomainException("Longitude must be between -180 and 180.");

        if (accuracyMeters is < 0)
            throw new DomainException("Accuracy must be non-negative.");

        Latitude = latitude;
        Longitude = longitude;
        AccuracyMeters = accuracyMeters;
        RecordedAt = recordedAt;
        ReceivedAt = DateTime.UtcNow;
    }
}