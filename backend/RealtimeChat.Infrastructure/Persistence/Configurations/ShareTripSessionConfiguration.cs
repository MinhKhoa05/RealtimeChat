using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ShareTripSessionConfiguration : BaseEntityConfiguration<ShareTripSession>
{
    public override void Configure(EntityTypeBuilder<ShareTripSession> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.TripTitle)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.DestinationLatitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.DestinationLongitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.EtaBaseLatitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.EtaBaseLongitude)
            .HasPrecision(9, 6);

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