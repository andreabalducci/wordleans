namespace Orleans.ChaosMonkey;

public class ChaosOptions
{
    private static int _counter;
    private readonly string _siloName;

    public ChaosOptions(string siloName)
    {
        _siloName = siloName;
    }

    private TimeSpan SimulatedNetworkDelay { get; set; } = TimeSpan.FromMilliseconds(200);
 
    public async Task ReadAsync(CancellationToken cancellationToken)
    {
        var i = Interlocked.Increment(ref _counter);
        if (i % 10 == 1)
        {
            throw new Exception($"{_siloName} #{i}!!!!");
        }

        await Task.Delay(SimulatedNetworkDelay, cancellationToken);
    }
}