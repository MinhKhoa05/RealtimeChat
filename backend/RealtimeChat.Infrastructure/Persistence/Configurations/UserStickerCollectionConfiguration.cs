using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class UserStickerCollectionConfiguration : BaseEntityConfiguration<UserStickerCollection>
{
    public override void Configure(EntityTypeBuilder<UserStickerCollection> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.CollectionId
        }).IsUnique();

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