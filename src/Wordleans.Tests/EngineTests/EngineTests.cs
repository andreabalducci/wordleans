using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Serilog;
using Wordleans.Api.Grains;
using Xunit;
using Xunit.Abstractions;

namespace Wordleans.Tests.EngineTests;

//[Collection(ClusterCollection.Name)]
public class EngineTests : IDisposable
{
    private readonly IClusterClient _client;
    private readonly ClusterFixture _fixture;

    public EngineTests(ITestOutputHelper helper)
    {
        // ChaosMonkeyCluster.Enabled = false;


        // workaround for showing logs in rider
        _fixture = new ClusterFixture(helper);
        _client = _fixture.Cluster!.Client;
    }

    [Fact(Skip = "long running")]
    public async Task RunGame()
    {
        var player = _client.GetGrain<IPlayer>("Player_1");

        var result = await player.EnterGuess("AEIOU");
        Assert.Equal("AEIOU", result.Word);
        Assert.False(result.IsValidWord);

        result = await player.EnterGuess("HELLO");
        Assert.True(result.IsValidWord);

        result = await player.EnterGuess("WOUND");
        Assert.True(result.IsValidWord);
        Assert.False(result.HasWon);

        result = await player.EnterGuess("CRANK");
        Assert.True(result.HasWon);
    }

//    [Fact(Skip = "long running")]
    [Fact]
    public async Task LoadTest()
    {
        string[][] guesses =
        {
            new[] { "HELLO", "HELLO", "WOUND", "WORLD", "HELLO", "CRANK" },
            new[] { "HELLO", "HELLO", "WOUND", "WORLD", "HELLO", "HELLO" },
            new[] { "WORLD", "CRANK" },
            new[] { "CRANK" }
        };

        var sw = new Stopwatch();
        sw.Start();
        int playing = 0;
        int won = 0;
        int lost = 0;
        int exceptions = 0;

        var tasks = Enumerable.Range(0, 100).Select(async i =>
        {
            try
            {
                var script = guesses[i % guesses.Length];
                var robot = _client.GetGrain<IRobot>($"Robot_{i:D3}");
                var player = $"Player_{i:D3}";
                var result = await robot.Play(player, script);
                int _ = result switch
                {
                    RobotPlayResult.Lost => Interlocked.Increment(ref lost),
                    RobotPlayResult.Playing => Interlocked.Increment(ref playing),
                    RobotPlayResult.Won => Interlocked.Increment(ref won),
                    _ => 0
                };
            }
            catch (Exception e)
            {
                Interlocked.Increment(ref exceptions);
                Log.Logger.Error(e,"Run {i} failed", i);
            }       
        });

        await Task.WhenAll(tasks);
        sw.Stop();

        Log.Logger.Information
        (
            "Time elapsed {elapsed}. Won {Won} Lost {Lost} Playing {Playing} Exceptions {Exceptions}",
            sw.Elapsed,
            won,
            lost,
            playing,
            exceptions
        );
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}