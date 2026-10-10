using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Infrastructure.Persistence.Configurations.ValueObjectConfigs;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class UserLiveLocationConfiguration : IEntityTypeConfiguration<UserLiveLocation>
{
    public void Configure(EntityTypeBuilder<UserLiveLocation> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.OwnsOne(x => x.Coordinate, geo =>
        {
            geo.ConfigureGeoCoordinate();
        });

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<UserLiveLocation>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}