using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace Orleans.ChaosMonkey;

public class SlowNetworkMiddleware
{
    private readonly ConnectionDelegate _next;
    private readonly SiloConnectionOptions _options;
    private readonly ChaosOptions _chaosOptions;
    private readonly ILogger _logger;

    public SlowNetworkMiddleware(string name,
        ConnectionDelegate next,
        SiloConnectionOptions options,
        ILoggerFactory loggerFactory,
        ChaosOptions chaosOptions)
    {
        _next = next;
        _options = options;
        _chaosOptions = chaosOptions;
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
            connection.Transport = new UnreliableDuplexPipe(originalTransport, _chaosOptions);
            await _next(connection);
        }
        finally
        {
            connection.Transport = originalTransport;
        }
    }
}