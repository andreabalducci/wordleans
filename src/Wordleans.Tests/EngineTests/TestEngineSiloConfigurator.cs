using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        siloBuilder.Configure<SiloConnectionOptions>(options =>
        {
            options.ConfigureSiloInboundConnection(connectionBuilder =>
            {
                AddMiddleware("Inbound", connectionBuilder, options);
            });
            options.ConfigureSiloOutboundConnection(connectionBuilder =>
            {
                AddMiddleware("Outbound", connectionBuilder, options);
            });
            options.ConfigureGatewayInboundConnection(connectionBuilder =>
            {
                AddMiddleware("Gateway", connectionBuilder, options);
            });
        });
    }

    private static void AddMiddleware(string name, IConnectionBuilder connectionBuilder, SiloConnectionOptions options)
    {
        var loggerFactory =
            connectionBuilder.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory ??
            NullLoggerFactory.Instance;
        connectionBuilder.Use(next =>
        {
            var middleware = new SlowNetworkMiddleware(name, next, options, loggerFactory);
            return middleware.OnConnectionAsync;
        });
    }
}