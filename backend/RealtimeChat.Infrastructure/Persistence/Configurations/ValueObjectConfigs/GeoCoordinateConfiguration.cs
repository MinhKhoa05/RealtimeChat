using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.ValueObjects;

namespace RealtimeChat.Infrastructure.Persistence.Configurations.ValueObjectConfigs;

public static class GeoCoordinateConfiguration
{
    public static OwnedNavigationBuilder<TEntity, GeoCoordinate>
        ConfigureGeoCoordinate<TEntity>(this OwnedNavigationBuilder<TEntity, GeoCoordinate> builder) where TEntity : class
    {
        builder.Property(x => x.Latitude)
            .HasPrecision(10, 7)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .HasPrecision(10, 7)
            .IsRequired();

        return builder;
    }
}