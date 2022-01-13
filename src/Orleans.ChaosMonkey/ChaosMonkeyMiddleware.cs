using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace Orleans.ChaosMonkey;

public class ChaosMonkeyMiddleware
{
    private readonly ConnectionDelegate _next;
    private readonly SiloConnectionOptions _options;
    private readonly ChaosOptions _chaosOptions;
    private readonly ILogger _logger;

    public ChaosMonkeyMiddleware(
        string name,
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
        var originalTransport = connection.Transport;

        try
        {
            connection.Transport = new UnreliableDuplexPipe(originalTransport, _chaosOptions, _logger);
            await _next(connection);
        }
        finally
        {
            connection.Transport = originalTransport;
        }
    }
}