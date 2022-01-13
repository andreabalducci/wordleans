using System.IO.Pipelines;

namespace Orleans.ChaosMonkey;

public class UnreliableDuplexPipe : IDuplexPipe
{
    public UnreliableDuplexPipe(IDuplexPipe wrapped, ChaosOptions options)
    {
        this.Output = wrapped.Output;
        this.Input = new UnreliablePipeReader(wrapped.Input, options);
    }

    public PipeReader Input { get; }
    public PipeWriter Output { get; }
}