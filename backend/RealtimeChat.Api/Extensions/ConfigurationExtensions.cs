namespace RealtimeChat.Api.Extensions;

public static class ConfigurationExtensions
{
    public static void ConfigureDatabaseConnectionString(this IConfiguration configuration)
    {
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? configuration["Database:Host"];
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? configuration["Database:Port"];
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? configuration["Database:Name"];
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? configuration["Database:User"];
        var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? configuration["Database:Password"];

        configuration["ConnectionStrings:Default"] = $"server={dbHost};port={dbPort};database={dbName};user={dbUser};password={dbPass};SslMode=None;";
    }
}
