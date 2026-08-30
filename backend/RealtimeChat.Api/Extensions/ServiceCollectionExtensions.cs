using RealtimeChat.BLL.Options;
using RealtimeChat.BLL.Services.Auth;
using RealtimeChat.BLL.Services.Users;
using RealtimeChat.DAL.Dapper;
using RealtimeChat.DAL.Transactions;
using RealtimeChat.DAL.Interfaces;
using RealtimeChat.DAL.Repositories;
using Microsoft.Extensions.Options;

namespace RealtimeChat.Api.Extensions;

public static class ServiceCollectionExtensions
{
    internal const string CorsPolicyName = "AllowReactApp";

    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppOptions>()
            .Bind(configuration.GetSection("App"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetSection("Cors"))
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection("Cors").Get<CorsOptions>() ?? new CorsOptions();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyName, policy =>
            {
                policy.WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddScoped<DapperContext>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("Default");
            return new DapperContext(new MySqlConnector.MySqlConnection(connectionString));
        });

        services.AddScoped<IDapperContext>(
            sp => sp.GetRequiredService<DapperContext>());

        services.AddScoped<ITransactionManager>(
            sp => sp.GetRequiredService<DapperContext>());

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<PasswordService>();
        services.AddScoped<TokenService>();

        return services;
    }

    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        return services;
    }

}
