using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class PostConfiguration : BaseEntityConfiguration<Post>
{
    public override void Configure(EntityTypeBuilder<Post> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.AuthorId)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnType("text")
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.AuthorId, x.CreatedAt });
    }
}
