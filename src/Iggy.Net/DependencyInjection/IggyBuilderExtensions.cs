using Iggy.Net.Contracts;
using Iggy.Net.Options;
using Iggy.Net.Stream;
using Iggy.Net.Topic;
using Microsoft.AspNetCore.Builder;
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

    public static IApplicationBuilder UseIggy(this IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var memoryStream = serviceScope.ServiceProvider.GetRequiredService<IMessageStream>();

            var streamTasks = TopicRegistrar.Options.Keys.Select(opt => 
                Task.Run(async () => memoryStream.CreateStreamAsync(new StreamRequest()
                {
                    Name = opt.Name,
                    StreamId = opt.Id
                })));

            Task.WhenAll(streamTasks);

            var topicTasks = TopicRegistrar.Options.Values.Select(opt =>
                Task.Run(async () => memoryStream.CreateTopicAsync(opt.StreamId, new TopicRequest()
                {
                    Name = opt.Name,
                    TopicId = opt.TopicId,
                    PartitionsCount = 1,
                })));

            Task.WhenAll(topicTasks);
        }

        return app;
    }
}

public static class IggyOptionsExtensions
{
    public static IggyOptions AddStream(this IggyOptions opts, Action<StreamOptions> streamOptions)
    {
        var options = new StreamOptions()
        {
            Id = StreamRegistrar.Options.Count,
            Name = $"stream-{StreamRegistrar.Options.Count}"
        };
        streamOptions.Invoke(options);
        StreamRegistrar.Options.Add(options);
        
        return opts;
    }
}

public static class IggyStreamOptionsExtensions
{
    public static StreamOptions AddTopic(this StreamOptions opts, Action<TopicOptions> streamOptions)
    {
        var options = new TopicOptions();
        streamOptions.Invoke(options);

        TopicRegistrar.Options.TryAdd(opts, options);
        return opts;
    }
} 