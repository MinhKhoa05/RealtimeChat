using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Conversations;

public static class DependencyInjection
{
    public static IServiceCollection AddConversations(this IServiceCollection services)
    {
        services.AddScoped<StartDirectConversationUseCase>();
        services.AddScoped<GetConversationUseCase>();
        services.AddScoped<GetConversationsUseCase>();
        services.AddScoped<SetConversationPinUseCase>();

        services.AddScoped<AddMemberUseCase>();
        services.AddScoped<CreateGroupUseCase>();
        services.AddScoped<GetGroupMembersUseCase>();
        services.AddScoped<KickMemberUseCase>();
        services.AddScoped<LeaveGroupUseCase>();
        services.AddScoped<TransferAdminUseCase>();
        services.AddScoped<DisbandGroupUseCase>();
        services.AddScoped<JoinGroupByInviteUseCase>();
        services.AddScoped<GetGroupByInviteCodeUseCase>();

        return services;
    }
}