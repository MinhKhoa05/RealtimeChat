namespace RealtimeChat.Domain.ValueObjects;

public record GeoCoordinate
{
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    private GeoCoordinate() { }

    public GeoCoordinate(decimal latitude, decimal longitude)
    {
        if (latitude is < -90 or > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude));

        if (longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude));

        Latitude = latitude;
        Longitude = longitude;
    }
}