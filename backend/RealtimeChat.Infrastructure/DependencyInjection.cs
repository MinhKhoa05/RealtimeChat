using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Interfaces;
using RealtimeChat.Infrastructure.Persistence;
using RealtimeChat.Infrastructure.Security;
using RealtimeChat.Infrastructure.Presence;
using RealtimeChat.Infrastructure.Caching;
using RealtimeChat.Infrastructure.Storage;

namespace RealtimeChat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.Parse("11.0.0-mariadb"));
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Security
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        // Presence
        services.AddSingleton<IPresenceTracker, InMemoryPresenceTracker>();

        // Caching
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        // Storage
        services.AddScoped<IFileStorage>(_ => new LocalFileStorage("storage"));

        return services;
    }
}