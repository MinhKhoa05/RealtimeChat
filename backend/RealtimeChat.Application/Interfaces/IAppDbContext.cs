using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Call> Calls { get; }
        DbSet<Conversation> Conversations { get; }
        DbSet<ConversationMember> ConversationMembers { get; }
        DbSet<Media> Medias { get; }
        DbSet<Message> Messages { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<Relationship> Relationships { get; }
        DbSet<User> Users { get; }
        DbSet<Sticker> Stickers { get; }
        DbSet<StickerCollection> StickerCollections { get; }
        DbSet<UserStickerCollection> UserStickerCollections { get; }
        DbSet<Mention> Mentions { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
