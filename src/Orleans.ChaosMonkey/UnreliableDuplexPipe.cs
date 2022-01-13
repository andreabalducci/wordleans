using System.IO.Pipelines;
using Microsoft.Extensions.Logging;

namespace Orleans.ChaosMonkey;

public class UnreliableDuplexPipe : IDuplexPipe
{
    public UnreliableDuplexPipe(IDuplexPipe wrapped, ChaosOptions options, ILogger logger)
    {
        this.Output = wrapped.Output;
        this.Input = new UnreliablePipeReader(wrapped.Input, options, logger);
    }

    public PipeReader Input { get; }
    public PipeWriter Output { get; }
}