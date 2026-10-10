using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;
using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Domain.Entities;

public class ShareTripSession : BaseEntity
{
    public long OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;
    public long ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    // Thông tin chuyển đi và điểm đến
    public string TripTitle { get; private set; } = null!;
    public GeoCoordinate Destination { get; private set; } = null!;

    // ETA gần nhất và vị trí dùng để kiểm soát tần suất tính ETA.
    public int? EtaMinutes { get; private set; }
    public DateTime? EtaCalculatedAt { get; private set; }
    public GeoCoordinate? LastEtaLocation { get; private set; }

    public ShareTripStatus Status { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    private static readonly TimeSpan DefaultSessionDuration = TimeSpan.FromHours(2);

    private ShareTripSession() { }

    public static ShareTripSession Create(long ownerId, long conversationId, string tripTitle, GeoCoordinate destination, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(tripTitle))
        {
            throw new DomainException("Trip title is required");
        }

        return new ShareTripSession
        {
            OwnerId = ownerId,
            ConversationId = conversationId,
            TripTitle = tripTitle,
            Destination = destination,
            Status = ShareTripStatus.Active,
            ExpiresAt = now.Add(DefaultSessionDuration),
        };
    }

    public void UpdateEta(int etaMinutes, GeoCoordinate etaLocation, DateTime now)
    {
        if (!IsActive(now))
        {
            throw new DomainException("Session is not active");
        }

        if (etaMinutes < 0)
        {
            throw new DomainException("ETA must be non-negative.");
        }

        EtaMinutes = etaMinutes;
        LastEtaLocation = etaLocation;
        EtaCalculatedAt = now;
    }

    public void Extend(TimeSpan duration, DateTime now)
    {
        if (Status != ShareTripStatus.Active)
        {
            throw new DomainException("Only active trips can be extended.");
        }

        if (ExpiresAt is null || ExpiresAt <= now)
        {
            throw new DomainException("Trip session has expired.");
        }

        if (duration <= TimeSpan.Zero)
        {
            throw new DomainException("Extension duration must be positive.");
        }

        ExpiresAt = ExpiresAt.Value.Add(duration);
    }

    public bool IsActive(DateTime now) => Status == ShareTripStatus.Active && now < ExpiresAt;

    public void End(DateTime now)
    {
        if (!IsActive(now))
        {
            throw new DomainException("Session is not active");
        }

        Status = ShareTripStatus.Ended;
    }
}