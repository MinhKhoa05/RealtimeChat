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

    public double DistanceTo(GeoCoordinate other)
    {
        const double earthRadius = 6_371_000;

        var lat1 = (double)Latitude * Math.PI / 180;
        var lat2 = (double)other.Latitude * Math.PI / 180;

        var deltaLat = lat2 - lat1;
        var deltaLon = ((double)other.Longitude - (double)Longitude)
            * Math.PI / 180;

        var a = Math.Pow(Math.Sin(deltaLat / 2), 2)
            + Math.Cos(lat1) * Math.Cos(lat2)
            * Math.Pow(Math.Sin(deltaLon / 2), 2);

        var c = 2 * Math.Atan2(
            Math.Sqrt(a),
            Math.Sqrt(1 - a));

        return earthRadius * c;
    }
}