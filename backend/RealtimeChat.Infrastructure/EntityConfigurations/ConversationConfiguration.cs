using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration
    : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("conversations");

        // Primary key
        builder.HasKey(x => x.Id);

        // Type
        builder.Property(x => x.Type)
            .IsRequired();

        // Name
        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired(false);

        // Avatar
        builder.HasOne(x => x.AvatarMedia)
            .WithMany()
            .HasForeignKey(x => x.AvatarMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        // CreatedAt
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // DisbandedAt
        builder.Property(x => x.DisbandedAt)
            .IsRequired(false);

        // Conversation -> Members
        builder.HasMany(x => x.Members)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId);

        // Conversation -> Messages
        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Conversation)
            .HasForeignKey(x => x.ConversationId);
    }
}