using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Wordleans.Api.Grains;

namespace Wordleans.Tests.EngineTests;

public class UnreliablePipeReader : PipeReader
{
    private static int _counter;
    private readonly PipeReader _pipeReaderImplementation;

    public UnreliablePipeReader(PipeReader pipeReaderImplementation)
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
        var i = Interlocked.Increment(ref _counter);
        if (i % 100 == 1)
        {
            Log.Logger.Warning("About to crash network #{i}", i);
            throw new Exception($"CHAOS #{i}!!!!");
        }

        await Task.Delay(Defaults.SimulatedNetworkDelay, cancellationToken);
        return await _pipeReaderImplementation.ReadAsync(cancellationToken);
    }

    public override bool TryRead(out ReadResult result)
    {
        return _pipeReaderImplementation.TryRead(out result);
    }
}