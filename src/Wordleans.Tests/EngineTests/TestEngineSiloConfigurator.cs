using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.TestingHost;
using Serilog;
using Wordleans.Api.Grains;
using Wordleans.Api.Services;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
public class SlowPipeReader : PipeReader
{
    private readonly PipeReader _pipeReaderImplementation;

    public SlowPipeReader(PipeReader pipeReaderImplementation)
    {
        _pipeReaderImplementation = pipeReaderImplementation;
    }

    public override void AdvanceTo(SequencePosition consumed)
    {
        _pipeReaderImplementation.AdvanceTo(consumed);
    }

    public override void AdvanceTo(SequencePosition consumed, SequencePosition examined)
    {
        _pipeReaderImplementation.AdvanceTo(consumed, examined);
    }

    public override void CancelPendingRead()
    {
        _pipeReaderImplementation.CancelPendingRead();
    }

    public override void Complete(Exception exception = null)
    {
        _pipeReaderImplementation.Complete(exception);
    }

    public override async ValueTask<ReadResult> ReadAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await Task.Delay(Defaults.SimulatedNetworkDelay, cancellationToken);
        return await _pipeReaderImplementation.ReadAsync(cancellationToken);
    }

    public override bool TryRead(out ReadResult result)
    {
        return _pipeReaderImplementation.TryRead(out result);
    }
}

public class SlowPipe : IDuplexPipe
{
    public SlowPipe(IDuplexPipe wrapped)
    {
        this.Output = wrapped.Output;
        this.Input = new SlowPipeReader(wrapped.Input);
    }

    public PipeReader Input { get; }
    public PipeWriter Output { get; }
}

public class SlowNetworkMiddleware
{
    private readonly ConnectionDelegate _next;
    private readonly SiloConnectionOptions _options;
    private readonly ILogger _logger;

    public SlowNetworkMiddleware(string name,
        ConnectionDelegate next,
        SiloConnectionOptions options,
        ILoggerFactory loggerFactory)
    {
        _next = next;
        _options = options;
        _logger = loggerFactory.CreateLogger($"{GetType().FullName}.{name}");
    }

    public async Task OnConnectionAsync(ConnectionContext connection)
    {
        // if (_logger.IsEnabled(LogLevel.Debug))
        // {
        //     var local = connection.LocalEndPoint.Serialize();
        //     var remote = connection.RemoteEndPoint.Serialize();
        //     _logger.LogDebug
        //     (
        //         "Connection Items {@Local} => {@Remote}", 
        //         local.ToString(), 
        //         remote
        //     );
        //     
        //     _logger.LogDebug("Connection transport {@Pipe}", connection.Transport);
        // }

        var originalTransport = connection.Transport;

        try
        {
            connection.Transport = new SlowPipe(originalTransport);
            await _next(connection);
        }
        finally
        {
            connection.Transport = originalTransport;
        }
    }
}