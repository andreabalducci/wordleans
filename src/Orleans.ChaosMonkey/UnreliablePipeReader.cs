using System.IO.Pipelines;
using Microsoft.Extensions.Logging;

namespace Orleans.ChaosMonkey;

public class UnreliablePipeReader : PipeReader
{
    private static int _counter;
    private readonly PipeReader _pipeReaderImplementation;
    private readonly ChaosOptions _options;
    private readonly ILogger _logger;

    public UnreliablePipeReader(PipeReader pipeReaderImplementation, ChaosOptions options, ILogger logger)
    {
        _pipeReaderImplementation = pipeReaderImplementation;
        _options = options;
        _logger = logger;
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