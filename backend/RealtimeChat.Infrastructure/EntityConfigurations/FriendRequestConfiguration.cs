using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class FriendRequestConfiguration
    : IEntityTypeConfiguration<FriendRequest>
{
    public void Configure(EntityTypeBuilder<FriendRequest> builder)
    {
        builder.ToTable("friend_requests");

        // Composite Primary Key
        builder.HasKey(x => new
        {
            x.SenderId,
            x.ReceiverId
        });

        // Sender
        builder.HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Receiver
        builder.HasOne(x => x.Receiver)
            .WithMany()
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Introduction
        builder.Property(x => x.Introduction)
            .HasMaxLength(500)
            .IsRequired(false);

        // CreatedAt
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}