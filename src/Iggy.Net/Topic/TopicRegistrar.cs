using System.Collections.Concurrent;
using Iggy.Net.Options;

namespace Iggy.Net.Topic;

public static class TopicRegistrar
{
    public static ConcurrentDictionary<StreamOptions, TopicOptions> Options = new ConcurrentDictionary<StreamOptions, TopicOptions>();
}