using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class StickerCollectionConfiguration
    : IEntityTypeConfiguration<StickerCollection>
{
    public void Configure(EntityTypeBuilder<StickerCollection> builder)
    {
        builder.ToTable("sticker_collections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Label)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.SharedKey)
            .IsRequired();

        builder.Property(x => x.Visibility)
            .HasConversion<string>()
            .HasMaxLength(20)
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