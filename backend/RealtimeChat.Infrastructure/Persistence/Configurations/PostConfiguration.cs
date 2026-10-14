using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class PostConfiguration : BaseEntityConfiguration<Post>
{
    public override void Configure(EntityTypeBuilder<Post> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnType("text")
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.CreatedAt });
    }
}
