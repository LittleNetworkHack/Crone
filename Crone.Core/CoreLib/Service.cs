namespace Crone;

#if NETSTANDARD2_1_OR_GREATER

public static partial class CoreLib
{
    public static IServiceCollection AddCoreOptions(this IServiceCollection services, Action<CoreOptions> options = null)
    {
        if (options != null)
        {
            services.Configure(options);
        }
        services.Configure<JsonSerializerOptions>(e => e.Converters.Add(new CoreObjectConverter()));
        return services;
    }
}

#endif