using Microsoft.EntityFrameworkCore;
using RealtimeChat.Infrastructure;


namespace RealtimeChat.Api.Setups
{
    public class DatabaseSetup
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default Connection");

            services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString));

            return services;
        }
    }
}