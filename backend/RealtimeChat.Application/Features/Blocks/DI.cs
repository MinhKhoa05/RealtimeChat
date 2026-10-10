using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Blocks;

public static class DependencyInjection
{
    public static IServiceCollection AddBlocks(this IServiceCollection services)
    {
        services.AddScoped<BlockUserUseCase>();
        services.AddScoped<UnblockUserUseCase>();
        services.AddScoped<GetBlockedUsersUseCase>();

        return services;
    }
}