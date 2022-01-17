using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Streams;
using Orleans.TestingHost;
using Serilog;
using Wordleans.Api.Services;
using Wordleans.Kernel.Silo;
using Wordleans.Kernel.Stats;

namespace Wordleans.Tests.EngineTests;

public class TestEngineSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.AddIncomingGrainCallFilter<IncomingGrainFilter>();
        
        siloBuilder.AddMemoryStreams<DefaultMemoryMessageBodySerializer>(StatsDefaults.StatsProvider, config =>
        {
            config.ConfigureStreamPubSub(StreamPubSubType.ImplicitOnly);
        });

        siloBuilder.ConfigureServices(services => { services.AddSingleton<IClock>(new FakeClock("2020-01-08")); });
        siloBuilder.ConfigureLogging((ctx, logging) => { logging.AddSerilog(); });
    }
}