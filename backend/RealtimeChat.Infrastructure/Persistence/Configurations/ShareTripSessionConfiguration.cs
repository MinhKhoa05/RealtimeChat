using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ShareTripSessionConfiguration
    : IEntityTypeConfiguration<ShareTripSession>
{
    public void Configure(EntityTypeBuilder<ShareTripSession> builder)
    {
        builder.ToTable("share_trip_sessions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TripTitle)
            .HasMaxLength(150)
            .IsRequired();

        // Tọa độ điểm đến.
        builder.Property(x => x.DestinationLatitude)
            .HasPrecision(9, 6);

        builder.Property(x => x.DestinationLongitude)
            .HasPrecision(9, 6);

        // ETA và vị trí tại lần tính ETA thành công gần nhất.
        builder.Property(x => x.EtaMinutes);

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