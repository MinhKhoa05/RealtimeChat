using Microsoft.Extensions.DependencyInjection;

namespace RealtimeChat.Application.Features.Stickers;

public static class DependencyInjection
{
    public static IServiceCollection AddStickers(this IServiceCollection services)
    {
        services.AddScoped<AddCollectionUseCase>();
        services.AddScoped<CreateCollectionUseCase>();
        services.AddScoped<CreateStickerUseCase>();

        services.AddScoped<GetAvailableCollectionsUseCase>();
        services.AddScoped<GetMyCollectionsUseCase>();
        services.AddScoped<GetSavedCollectionsUseCase>();

        services.AddScoped<RegenerateSharedKeyUseCase>();
        services.AddScoped<UpdateCollectionUseCase>();
        services.AddScoped<UpdateStickerUseCase>();

        return services;
    }
}