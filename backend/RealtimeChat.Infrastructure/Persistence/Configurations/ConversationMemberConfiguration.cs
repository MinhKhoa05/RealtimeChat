using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ConversationMemberConfiguration : BaseEntityConfiguration<ConversationMember>
{
    public override void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        base.Configure(builder);

        builder.HasIndex(x => new
        {
            x.ConversationId,
            x.MemberId
        }).IsUnique();

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.LastReadMessage)
            .WithMany()
            .HasForeignKey(x => x.LastReadMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}