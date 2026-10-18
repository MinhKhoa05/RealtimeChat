using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.HasOne(l => l.Post)
            .WithMany()
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasKey(l => new { l.PostId, l.UserId });
    }
}