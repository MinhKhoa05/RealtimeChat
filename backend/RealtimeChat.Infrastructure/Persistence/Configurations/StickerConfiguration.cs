using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class StickerConfiguration : BaseEntityConfiguration<Sticker>
{
    public override void Configure(EntityTypeBuilder<Sticker> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Label)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.Collection)
            .WithMany(x => x.Stickers)
            .HasForeignKey(x => x.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.PublicId)
            .IsRequired();

        builder.HasOne(x => x.Media)
            .WithMany()
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CollectionId);
        builder.HasIndex(x => x.MediaId);
        builder.HasIndex(x => x.PublicId)
            .IsUnique();
    }
}