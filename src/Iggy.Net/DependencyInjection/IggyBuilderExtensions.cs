using Iggy.Net.Options;
using Iggy.Net.Stream;
using Microsoft.Extensions.DependencyInjection;

namespace Iggy.Net.DependencyInjection;

public static class IggyBuilderExtensions
{
    public static IServiceCollection AddIggy(this IServiceCollection services)
    {
        return services.AddIggy(setupAction: null);
    }

    public static IServiceCollection AddIggy(this IServiceCollection services, Action<IggyOptions> setupAction = null)
    {
        services.ConfigureIggy(setupAction);

        services.AddHttpClient<IMessageStream, HttpMessageStream>((provider, client) =>
        {
            var options = provider.GetRequiredService<IggyOptions>();

            client.BaseAddress = new Uri(options.BaseAdress);
        });
        return services;
    }

    public static void ConfigureIggy(this IServiceCollection services, Action<IggyOptions> setupAction)
    {
        var options = new IggyOptions();
        setupAction?.Invoke(options);
        services.AddSingleton(options);
    }
}

public static class IggyOptionsExtensions
{
    public static IggyOptions AddStream(this IggyOptions opts, Action<StreamOptions> streamOptions)
    {
        var options = new StreamOptions();
        streamOptions.Invoke(options);
        
        
        return opts;
    }
}

public static class IggyStreamOptionsExtensions
{
    public static StreamOptions AddTopic(this StreamOptions opts, Action<TopicOptions> streamOptions)
    {
        var options = new TopicOptions();
        streamOptions.Invoke(options);
        
        
        return opts;
    }
} 