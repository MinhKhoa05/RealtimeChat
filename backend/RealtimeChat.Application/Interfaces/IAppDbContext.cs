using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Call> Calls { get; }
        DbSet<Conversation> Conversations { get; }
        DbSet<ConversationMember> ConversationMembers { get; }
        DbSet<FriendRequest> FriendRequests { get; }
        DbSet<Friendship> Friendships { get; }
        DbSet<Media> Medias { get; }
        DbSet<Message> Messages { get; }
        DbSet<RefreshToken> RefreshTokens {get; }
        DbSet<User> Users { get; }
        DbSet<UserBlock> UserBlocks { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
