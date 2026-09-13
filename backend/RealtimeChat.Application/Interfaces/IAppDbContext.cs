using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }

        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
