using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Orleans.Configuration;
using Orleans.Hosting;

namespace Orleans.ChaosMonkey;

public class SiloChaosConfigurator
{
    private static int _counter;

    public void Configure(ISiloBuilder siloBuilder)
    {
        var chaosOptions = new ChaosOptions($"Silo {_counter++}");
        siloBuilder.Configure<SiloMessagingOptions>(o =>
        {
            o.SystemResponseTimeout = TimeSpan.FromSeconds(5);
        });

        siloBuilder.Configure<SiloConnectionOptions>(siloConnectionOptions =>
        {
            siloConnectionOptions.ConfigureSiloInboundConnection(connectionBuilder =>
            {
                AddMiddleware("Inbound", connectionBuilder, siloConnectionOptions, chaosOptions);
            });
            siloConnectionOptions.ConfigureSiloOutboundConnection(connectionBuilder =>
            {
                AddMiddleware("Outbound", connectionBuilder, siloConnectionOptions, chaosOptions);
            });
            siloConnectionOptions.ConfigureGatewayInboundConnection(connectionBuilder =>
            {
                AddMiddleware("Gateway", connectionBuilder, siloConnectionOptions, chaosOptions);
            });
        });
    }

    private static void AddMiddleware(
        string name,
        IConnectionBuilder connectionBuilder,
        SiloConnectionOptions options,
        ChaosOptions chaosOptions)
    {
        var loggerFactory =
            connectionBuilder.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory ??
            NullLoggerFactory.Instance;

        connectionBuilder.Use(next =>
        {
            var middleware = new ChaosMonkeyMiddleware(name, next, options, loggerFactory, chaosOptions);
            return middleware.OnConnectionAsync;
        });
    }
}