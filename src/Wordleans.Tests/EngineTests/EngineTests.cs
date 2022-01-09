using System;
using System.Diagnostics;
using System.Linq;
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
        // workaround for showing logs in rider
        _fixture = new ClusterFixture(helper);
        _client = _fixture.Cluster.Client;
    }
    
    [Fact]
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

    [Fact(Skip = "incomplete")]
    public async Task LoadTest()
    {
        var sw = new Stopwatch();
        sw.Start();
        var tasks = Enumerable.Range(0, 100).Select(async i =>
        {
            var player = _client.GetGrain<IPlayer>($"Player_{i}");
            await player.EnterGuess("UUUUU");
        });

        await Task.WhenAll(tasks);
        sw.Stop();
        
        Log.Logger.Information("Time elapsed {elapsed}", sw.Elapsed);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}