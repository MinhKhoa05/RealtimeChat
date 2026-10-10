using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class MentionConfiguration : BaseEntityConfiguration<Mention>
{
    public override void Configure(EntityTypeBuilder<Mention> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Message)
            .WithMany(x => x.Mentions)
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Start)
            .IsRequired();

        builder.Property(x => x.Length)
            .IsRequired();
    }
}