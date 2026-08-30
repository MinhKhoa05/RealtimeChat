using RealtimeChat.DAL.Entities;

namespace RealtimeChat.DAL.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long userId);
}
