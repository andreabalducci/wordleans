using System.IO.Pipelines;

namespace Orleans.ChaosMonkey;

public class UnreliablePipeReader : PipeReader
{
    private static int _counter;
    private readonly PipeReader _pipeReaderImplementation;
    private readonly ChaosOptions _options;

    public UnreliablePipeReader(PipeReader pipeReaderImplementation, ChaosOptions options)
    {
        _pipeReaderImplementation = pipeReaderImplementation;
        _options = options;
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
        await _options.ReadAsync(cancellationToken);
        return await _pipeReaderImplementation.ReadAsync(cancellationToken);
    }

    public override bool TryRead(out ReadResult result)
    {
        return _pipeReaderImplementation.TryRead(out result);
    }
}