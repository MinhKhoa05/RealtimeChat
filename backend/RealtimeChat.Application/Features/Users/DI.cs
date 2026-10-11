using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsers(this IServiceCollection services)
    {
        services.AddScoped<GetCurrentUserUseCase>();
        services.AddScoped<GetUserDetailUseCase>();
        services.AddScoped<GetUserSummariesUseCase>();
        services.AddScoped<SearchUsersUseCase>();

        return services;
    }
}