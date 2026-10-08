using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Application.Exceptions;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Call> Calls => Set<Call>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
        public DbSet<Media> Medias => Set<Media>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Relationship> Relationships => Set<Relationship>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Sticker> Stickers => Set<Sticker>();
        public DbSet<StickerCollection> StickerCollections => Set<StickerCollection>();
        public DbSet<UserStickerCollection> UserStickerCollections => Set<UserStickerCollection>();
        public DbSet<Mention> Mentions => Set<Mention>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsDuplicateKey(ex))
            {
                throw new DuplicateKeyException(ex);
            }
        }

        private static bool IsDuplicateKey(DbUpdateException ex)
        {
            return ex.InnerException is MySqlException
            {
                Number: 1062
            };
        }
    }
}
