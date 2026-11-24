using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class UserStickerCollectionConfiguration : IEntityTypeConfiguration<UserStickerCollection>
{
    public void Configure(EntityTypeBuilder<UserStickerCollection> builder)
    {
        builder.HasKey(x => new
        {
            x.UserId,
            x.CollectionId
        });

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Collection)
            .WithMany()
            .HasForeignKey(x => x.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CollectionId);
    }
}