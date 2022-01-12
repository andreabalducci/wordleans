using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace Wordleans.Tests.EngineTests;

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
            connection.Transport = new UnreliableDuplexPipe(originalTransport);
            await _next(connection);
        }
        finally
        {
            connection.Transport = originalTransport;
        }
    }
}