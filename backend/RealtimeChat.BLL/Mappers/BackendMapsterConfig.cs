using Mapster;

namespace FullStackFoundation.BLL.Mappers;

internal static class BackendMapsterConfig
{
    public static readonly TypeAdapterConfig Config = Create();

    private static TypeAdapterConfig Create()
    {
        var config = new TypeAdapterConfig();



        return config;
    }
}
