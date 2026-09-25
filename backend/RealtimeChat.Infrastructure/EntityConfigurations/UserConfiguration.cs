using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence.Configurations;

public class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        // Name
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Email
        builder.Property(x => x.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        // Password
        builder.Property(x => x.Password)
            .HasMaxLength(255)
            .IsRequired();

        // Avatar
        builder.HasOne(x => x.AvatarMedia)
            .WithMany()
            .HasForeignKey(x => x.AvatarMediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}