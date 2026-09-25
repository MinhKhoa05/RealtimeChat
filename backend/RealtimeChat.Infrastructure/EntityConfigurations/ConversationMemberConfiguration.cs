using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ConversationMemberConfiguration
    : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.ToTable("conversation_members");

        // Composite Primary Key
        builder.HasKey(x => new
        {
            x.ConversationId,
            x.MemberId
        });

        // Conversation
        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Member
        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Last read message
        builder.HasOne(x => x.LastReadMessage)
            .WithMany()
            .HasForeignKey(x => x.LastReadMessageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.IsPinned)
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .IsRequired();
    }
}