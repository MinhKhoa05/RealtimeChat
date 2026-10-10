using RealtimeChat.Domain.Enums;
using RealtimeChat.Domain.Exceptions;

namespace RealtimeChat.Domain.Entities;

public class ShareTripSession : BaseEntity
{
    public long OwnerId { get; private set; }
    public User Owner { get; private set; } = null!;
    public long ConversationId { get; private set; }
    public Conversation Conversation { get; private set; } = null!;

    // Thông tin chuyển đi và điểm đến
    public string TripTitle { get; private set; } = null!;
    public decimal DestinationLatitude { get; private set; }
    public decimal DestinationLongitude { get; private set; }

    // ETA gần nhất và vị trí dùng để kiểm soát tần suất tính ETA.
    public int? EtaMinutes { get; private set; }
    public DateTime? EtaCalculatedAt { get; private set; }
    public decimal? EtaBaseLatitude { get; private set; }
    public decimal? EtaBaseLongitude { get; private set; }

    public ShareTripStatus Status { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    private static readonly TimeSpan DefaultSessionDuration = TimeSpan.FromHours(2);

    private ShareTripSession() { }

    public static ShareTripSession Create(long ownerId, long conversationId, string tripTitle, decimal destinationLatitude, decimal destinationLongitude)
    {
        if (string.IsNullOrWhiteSpace(tripTitle))
        {
            throw new DomainException("Trip title is required");
        }

        if (destinationLatitude is < -90 or > 90)
        {
            throw new DomainException("Latitude must be between -90 and 90.");
        }

        if (destinationLongitude is < -180 or > 180)
        {
            throw new DomainException("Longitude must be between -180 and 180.");
        }

        return new ShareTripSession
        {
            OwnerId = ownerId,
            ConversationId = conversationId,
            DestinationLatitude = destinationLatitude,
            DestinationLongitude = destinationLongitude,
            TripTitle = tripTitle,
            Status = ShareTripStatus.Active,
            ExpiresAt = DateTime.UtcNow.Add(DefaultSessionDuration),
        };
    }

    public void UpdateEta(int etaMinutes, decimal baseLatitude, decimal baseLongitude)
    {
        if (!IsActive())
        {
            throw new DomainException("Session is not active");
        }

        if (etaMinutes < 0)
            throw new DomainException("ETA must be non-negative.");

        if (baseLatitude is < -90 or > 90)
            throw new DomainException("Latitude must be between -90 and 90.");

        if (baseLongitude is < -180 or > 180)
            throw new DomainException("Longitude must be between -180 and 180.");

        EtaMinutes = etaMinutes;
        EtaBaseLatitude = baseLatitude;
        EtaBaseLongitude = baseLongitude;
        EtaCalculatedAt = DateTime.UtcNow;
    }

    public void Extend(TimeSpan duration)
    {
        if (Status != ShareTripStatus.Active)
        {
            throw new DomainException("Only active trips can be extended.");
        }

        if (ExpiresAt is null || ExpiresAt <= DateTime.UtcNow)
        {
            throw new DomainException("Trip session has expired.");
        }

        if (duration <= TimeSpan.Zero)
        {
            throw new DomainException("Extension duration must be positive.");
        }

        ExpiresAt = ExpiresAt.Value.Add(duration);
    }

    public bool IsActive() => Status == ShareTripStatus.Active && DateTime.UtcNow < ExpiresAt;

    public void End()
    {
        if (!IsActive())
        {
            throw new DomainException("Session is not active");
        }

        Status = ShareTripStatus.Ended;
    }
}