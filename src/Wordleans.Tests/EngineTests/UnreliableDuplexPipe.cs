using System.IO.Pipelines;

namespace Wordleans.Tests.EngineTests;

public class UnreliableDuplexPipe : IDuplexPipe
{
    public UnreliableDuplexPipe(IDuplexPipe wrapped)
    {
        this.Output = wrapped.Output;
        this.Input = new UnreliablePipeReader(wrapped.Input);
    }

    public PipeReader Input { get; }
    public PipeWriter Output { get; }
}