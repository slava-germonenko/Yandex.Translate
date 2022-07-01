using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Yandex.Translate.Core;

namespace Yandex.Translate.Extensions.DependencyInjection;

public static class TranslateClientServiceCollectionExtensions
{
    public static void AddTranslateClient(
        this IServiceCollection services,
        string apiKey,
        ServiceLifetime clientServiceLifetime = ServiceLifetime.Scoped
    )
    {
        services.AddHttpClient();

        var translateServiceDescriptor = new ServiceDescriptor(
            typeof(TranslateClient),
            factory => new TranslateClient(
                factory.GetRequiredService<HttpClient>(),
                new TranslateClientConfiguration(apiKey)
             ),
            clientServiceLifetime
        );
        
        services.TryAdd(translateServiceDescriptor);
    }
    
    public static void AddTranslateClient(
        this IServiceCollection services,
        TranslateClientConfiguration configuration,
        ServiceLifetime clientServiceLifetime = ServiceLifetime.Scoped
    )
    {
        services.AddHttpClient();

        var translateServiceDescriptor = new ServiceDescriptor(
            typeof(TranslateClient),
            factory => new TranslateClient(
                factory.GetRequiredService<HttpClient>(),
                configuration
            ),
            clientServiceLifetime
        );
        
        services.TryAdd(translateServiceDescriptor);
    }
}