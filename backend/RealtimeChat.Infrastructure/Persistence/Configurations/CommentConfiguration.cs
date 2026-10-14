using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : BaseEntityConfiguration<Comment>
{
    public override void Configure(EntityTypeBuilder<Comment> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Content)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(c => c.DeletedAt);

        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.PostId, c.CreatedAt });
        builder.HasIndex(c => c.DeletedAt);
    }
}
