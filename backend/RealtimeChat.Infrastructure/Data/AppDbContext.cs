using Microsoft.EntityFrameworkCore;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Infrastructure.Data
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Call> Calls => Set<Call>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
        public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
        public DbSet<Friendship> Friendships => Set<Friendship>();
        public DbSet<Media> Medias => Set<Media>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<UserBlock> UserBlocks => Set<UserBlock>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
