using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Posts;

public static class DependencyInjection
{
    public static IServiceCollection AddPosts(this IServiceCollection services)
    {
        services.AddScoped<CreatePostUseCase>();
        services.AddScoped<DeletePostUseCase>();
        services.AddScoped<LikePostUseCase>();
        services.AddScoped<UnlikePostUseCase>();
        services.AddScoped<CreateCommentUseCase>();

        return services;
    }
}