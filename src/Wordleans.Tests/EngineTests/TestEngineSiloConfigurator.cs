using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Orleans.ChaosMonkey;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.TestingHost;
using Serilog;
using Wordleans.Api.Services;

namespace Wordleans.Tests.EngineTests;

public class TestEngineSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.ConfigureServices(services => { services.AddSingleton<IClock>(new FakeClock("2020-01-08")); });
        siloBuilder.ConfigureLogging((ctx, logging) => { logging.AddSerilog(); });
    }
}