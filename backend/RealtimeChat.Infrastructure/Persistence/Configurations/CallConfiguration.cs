using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class CallConfiguration : BaseEntityConfiguration<Call>
{
    public override void Configure(EntityTypeBuilder<Call> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Caller)
            .WithMany()
            .HasForeignKey(x => x.CallerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Receiver)
            .WithMany()
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.CallerId);
        builder.HasIndex(x => x.ReceiverId);
    }
}