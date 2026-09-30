using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Application.Features.Auth;
using RealtimeChat.Application.Features.Blocks;
using RealtimeChat.Application.Features.Friends;
using RealtimeChat.Application.Features.Users;

namespace RealtimeChat.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Auths
        services.AddScoped<RegisterUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUseCase>();
        services.AddScoped<LogoutAllUseCase>();
        services.AddScoped<ChangePasswordUseCase>();

        // Users
        services.AddScoped<GetCurrentUserUseCase>();
        services.AddScoped<GetUserProfileUseCase>();
        services.AddScoped<SearchUsersUseCase>();

        // Friends
        services.AddScoped<SendFriendRequestUseCase>();
        services.AddScoped<AcceptFriendRequestUseCase>();
        services.AddScoped<RejectFriendRequestUseCase>();
        services.AddScoped<RevokeFriendRequestUseCase>();
        services.AddScoped<RemoveFriendUseCase>();
        services.AddScoped<GetFriendsUseCase>();
        services.AddScoped<GetSentFriendRequestsUseCase>();
        services.AddScoped<GetReceivedFriendRequestsUseCase>();

        // Blocks
        services.AddScoped<BlockUserUseCase>();
        services.AddScoped<UnBlockUserUseCase>();
        services.AddScoped<GetBlockedUsersUseCase>();

        return services;
    }
}