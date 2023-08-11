using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Iggy.Net.Contracts;
using Iggy.Net.Enums;
using Iggy.Net.Options;
using Iggy.Net.Stream;
using Microsoft.Extensions.Logging;

namespace Iggy.Net.Topic;

public static class TopicRegistrar
{
    public static ConcurrentDictionary<StreamOptions, TopicOptions> Options =
        new ConcurrentDictionary<StreamOptions, TopicOptions>();
}

internal class BackgroundServiceT : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IMessageStream _messageStream;
    private readonly ILogger<BackgroundServiceT> _logger;

    public BackgroundServiceT(
        IMessageStream messageStream,
        ILogger<BackgroundServiceT> logger)
    {
        _messageStream = messageStream;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pollMessagesAsync = await _messageStream.PollMessagesAsync(new MessageFetchRequest()
                {
                    Consumer = Consumer.New(1),
                    Count = 10,
                    AutoCommit = true,
                    PollingStrategy = MessagePolling.Next,
                    StreamId = 1,
                    TopicId = 1,
                    Value = 0,
                    PartitionId = 1
                });

                await foreach (var messageResponse in pollMessagesAsync)
                {
                    _logger.LogInformation("Got message: {message}", JsonSerializer.Serialize(messageResponse));
                    _logger.LogInformation("Payload is : {message}", Encoding.UTF8.GetString(messageResponse.Payload));
                }
            }
            finally
            {
                // await Task.Delay(TimeSpan.FromMilliseconds(300));
            }
            
        }
    }
}