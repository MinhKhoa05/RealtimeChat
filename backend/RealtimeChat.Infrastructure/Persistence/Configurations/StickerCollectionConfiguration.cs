using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class StickerCollectionConfiguration : BaseEntityConfiguration<StickerCollection>
{
    public override void Configure(EntityTypeBuilder<StickerCollection> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Label)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SharedKey)
            .IsRequired();

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => x.SharedKey)
            .IsUnique();
    }
}