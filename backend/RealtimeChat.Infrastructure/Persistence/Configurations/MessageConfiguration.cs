using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class MessageConfiguration
    : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(x => x.Id);

        // Type
        builder.Property(x => x.Type)
            .IsRequired();

        // Content
        builder.Property(x => x.Content)
            .IsRequired(false);

        // Conversation
        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Sender
        builder.HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.SetNull);

        // Media
        builder.HasOne(x => x.Media)
            .WithMany()
            .HasForeignKey(x => x.MediaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Call
        builder.HasOne(x => x.Call)
            .WithMany()
            .HasForeignKey(x => x.CallId)
            .OnDelete(DeleteBehavior.SetNull);

        // CreatedAt
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // RecalledAt
        builder.Property(x => x.RecalledAt)
            .IsRequired(false);

        // Index
        builder.HasIndex(x => new
        {
            x.ConversationId,
            x.CreatedAt
        });
    }
}