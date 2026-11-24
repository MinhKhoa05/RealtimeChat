using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.HasOne(x => x.AvatarMedia)
            .WithMany()
            .HasForeignKey(x => x.AvatarMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.DirectKey)
            .IsUnique();

        builder.HasIndex(x => x.InviteCode)
            .IsUnique();

        builder.HasMany(x => x.Members)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId);

    }
}