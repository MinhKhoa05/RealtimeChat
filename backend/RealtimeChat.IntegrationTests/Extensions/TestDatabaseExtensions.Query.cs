using RealtimeChat.DAL.Dapper;
using RealtimeChat.DAL.Entities;
using RealtimeChat.IntegrationTests.Infrastructure;

namespace RealtimeChat.IntegrationTests.Extensions;

public static partial class TestDatabaseExtensions
{
    public static Task<User?> GetUserAsync(
        this CustomWebApplicationFactory factory,
        long userId)
    {
        return factory.WithDbAsync(db =>
            db.GetByIdAsync<User>(userId));
    }

    public static Task<User?> GetUserByEmailAsync(
        this CustomWebApplicationFactory factory,
        string email)
    {
        return factory.WithDbAsync(db =>
            db.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM users WHERE email = @Email",
                new {Email = email}));
    }

}
