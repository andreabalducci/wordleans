using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.TestingHost;
using Wordleans.Kernel.Stats;

namespace Wordleans.Tests.EngineTests;

public class TestClientConfigurator : IClientBuilderConfigurator
{
    public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
    {
        clientBuilder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>(StatsDefaults.StatsProvider, config =>
        {
            config.ConfigureStreamPubSub(StreamPubSubType.ImplicitOnly);
        });
    }
}