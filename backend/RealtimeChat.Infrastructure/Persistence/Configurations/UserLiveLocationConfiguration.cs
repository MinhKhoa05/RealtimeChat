using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class UserLiveLocationConfiguration
    : IEntityTypeConfiguration<UserLiveLocation>
{
    public void Configure(EntityTypeBuilder<UserLiveLocation> builder)
    {
        builder.ToTable("user_live_locations");

        builder.HasKey(x => x.UserId);

        builder.Property(x => x.Latitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.Longitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.AccuracyMeters);

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<UserLiveLocation>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}