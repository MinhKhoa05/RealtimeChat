using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class UserBlockConfiguration
    : IEntityTypeConfiguration<UserBlock>
{
    public void Configure(EntityTypeBuilder<UserBlock> builder)
    {
        builder.ToTable("user_blocks");

        // Composite Primary Key
        builder.HasKey(x => new
        {
            x.BlockerId,
            x.BlockedUserId
        });

        // Blocker
        builder.HasOne(x => x.Blocker)
            .WithMany()
            .HasForeignKey(x => x.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Blocked User
        builder.HasOne(x => x.BlockedUser)
            .WithMany()
            .HasForeignKey(x => x.BlockedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // CreatedAt
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}