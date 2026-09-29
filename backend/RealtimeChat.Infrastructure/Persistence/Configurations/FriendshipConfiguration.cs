using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class FriendshipConfiguration
    : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("friendships");

        // Composite Primary Key
        builder.HasKey(x => new
        {
            x.UserId,
            x.FriendId
        });

        // User
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Friend
        builder.HasOne(x => x.Friend)
            .WithMany()
            .HasForeignKey(x => x.FriendId)
            .OnDelete(DeleteBehavior.Restrict);

        // CreatedAt
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}