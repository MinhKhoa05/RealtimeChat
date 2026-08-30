using RealtimeChat.DAL.Dapper;
using RealtimeChat.DAL.Entities;
using RealtimeChat.DAL.Interfaces;

namespace RealtimeChat.DAL.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDapperContext _db;

    public UserRepository(IDapperContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(long userId) => _db.GetByIdAsync<User>(userId);
    
}
