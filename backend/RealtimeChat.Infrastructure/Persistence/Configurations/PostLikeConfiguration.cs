using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class PostLikeConfiguration : BaseEntityConfiguration<PostLike>
{
    public override void Configure(EntityTypeBuilder<PostLike> builder)
    {
        base.Configure(builder);

        builder.HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // A user can like a post only once.
        builder.HasIndex(l => new { l.PostId, l.UserId })
            .IsUnique();

        builder.HasIndex(l => new { l.UserId, l.CreatedAt });
    }
}