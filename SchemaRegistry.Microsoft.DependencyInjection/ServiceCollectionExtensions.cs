using System.Reflection;
using SchemaRegistry;
using Microsoft.Extensions.DependencyInjection;

namespace SchemaRegistry.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSchemaRegistry(
        this IServiceCollection services, 
        Action<SchemaRegistryConfiguration> configure)
    {
        SchemaRegistryConfiguration config = new();
        configure(config);
        services.AddSingleton(config);
        services.AddSingleton<IDataStore>(config.DataStore);
        services.AddSingleton<IRegistry, Registry>();
        return services;
    }
}