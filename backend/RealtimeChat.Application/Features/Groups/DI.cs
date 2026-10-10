using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Groups;

public static class DependencyInjection
{
    public static IServiceCollection AddGroups(this IServiceCollection services)
    {
        services.AddScoped<AddMemberUseCase>();
        services.AddScoped<CreateGroupUseCase>();
        services.AddScoped<GetGroupMembersUseCase>();
        services.AddScoped<GetGroupUseCase>();
        services.AddScoped<GetMyGroupsUseCase>();
        services.AddScoped<KickMemberUseCase>();
        services.AddScoped<LeaveGroupUseCase>();
        services.AddScoped<TransferAdminUseCase>();
        services.AddScoped<DisbandGroupUseCase>();

        return services;
    }
}