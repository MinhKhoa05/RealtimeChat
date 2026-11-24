using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class ConversationMemberConfiguration : IEntityTypeConfiguration<ConversationMember>
{
    public void Configure(EntityTypeBuilder<ConversationMember> builder)
    {
        builder.HasKey(x => new
        {
            x.ConversationId,
            x.MemberId
        });

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.InvitedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}