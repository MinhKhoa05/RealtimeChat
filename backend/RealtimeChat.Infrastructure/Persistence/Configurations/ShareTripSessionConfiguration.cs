using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Infrastructure.Persistence.Configurations.ValueObjectConfigs;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ShareTripSessionConfiguration : BaseEntityConfiguration<ShareTripSession>
{
    public override void Configure(EntityTypeBuilder<ShareTripSession> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TripTitle)
            .HasMaxLength(150)
            .IsRequired();

        builder.OwnsOne(x => x.Destination, geo =>
        {
            geo.ConfigureGeoCoordinate();
        });

        builder.OwnsOne(x => x.LastEtaLocation, geo =>
        {
            geo.ConfigureGeoCoordinate();
        });

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Conversation)
            .WithMany()
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}