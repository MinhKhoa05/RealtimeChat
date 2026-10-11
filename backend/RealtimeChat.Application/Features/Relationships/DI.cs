using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Relationships;

public static class DependencyInjection
{
    public static IServiceCollection AddRelationships(this IServiceCollection services)
    {
        services.AddScoped<BlockUserUseCase>();
        services.AddScoped<UnblockUserUseCase>();
        services.AddScoped<GetBlockedUsersUseCase>();

        services.AddScoped<SendFriendRequestUseCase>();
        services.AddScoped<AcceptFriendRequestUseCase>();
        services.AddScoped<RejectFriendRequestUseCase>();
        services.AddScoped<RevokeFriendRequestUseCase>();
        services.AddScoped<GetSentFriendRequestsUseCase>();
        services.AddScoped<GetReceivedFriendRequestsUseCase>();

        services.AddScoped<RemoveFriendUseCase>();
        services.AddScoped<GetFriendsUseCase>();

        return services;
    }
}