using RealtimeChat.DAL.Dapper;
using RealtimeChat.DAL.Entities;
using RealtimeChat.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.IntegrationTests.Extensions;

public static partial class TestDatabaseExtensions
{
    public const string DefaultUserPassword = "Password123!";

    public static string UniqueCode(int length = 8) => Guid.NewGuid().ToString("N")[..length];
    public static string UniqueEmail(string prefix = "user") => $"{prefix}-{UniqueCode()}@test.local";

    public static Task<User> SeedUserAsync(
        this CustomWebApplicationFactory factory,
        Action<User>? customize = null)
    {
        var unique = UniqueCode(10);
        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
        };

        customize?.Invoke(user);

        return factory.InsertSeedAsync(user, (x, id) => x.Id = id);
    }

    private static Task<T> InsertSeedAsync<T>(
        this CustomWebApplicationFactory factory,
        T entity,
        Action<T, long> setId)
        where T : class
    {
        return factory.WithDbAsync(async db =>
        {
            var id = await db.InsertAsync(entity);
            if (id <= 0)
            {
                throw new InvalidOperationException($"{typeof(T).Name} insert returned no id.");
            }

            setId(entity, id);
            return entity;
        });
    }

    private static async Task<T> WithDbAsync<T>(
        this CustomWebApplicationFactory factory,
        Func<IDapperContext, Task<T>> action)
    {
        using var scope = factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<IDapperContext>();
        return await action(database);
    }
}
