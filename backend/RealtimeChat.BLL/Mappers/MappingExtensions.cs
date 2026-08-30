using Mapster;

namespace FullStackFoundation.BLL.Mappers;

public static class MappingExtensions
{
    public static TDestination MapTo<TDestination>(this object source)
    {
        return source.Adapt<TDestination>(BackendMapsterConfig.Config);
    }

    public static TDestination ApplyTo<TSource, TDestination>(this TSource source, TDestination destination)
        where TSource : class
        where TDestination : class
    {
        source.Adapt(destination, BackendMapsterConfig.Config);
        return destination;
    }
}
