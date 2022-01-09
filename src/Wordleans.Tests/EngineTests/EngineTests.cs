using System;
using System.Threading.Tasks;
using Orleans;
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
        var riddleId = await player.GetTodaysRiddleId();
        var riddle = _client.GetGrain<IWordRiddle>(riddleId);

        var result = await riddle.Guess("AEIOU");
        Assert.Equal("AEIOU", result.Word);
        Assert.False(result.IsValidWord);

        result = await riddle.Guess("HELLO");
        Assert.True(result.IsValidWord);

        result = await riddle.Guess("WOUND");
        Assert.True(result.IsValidWord);
        Assert.False(result.HasWon);


        result = await riddle.Guess("CRANK");
        Assert.True(result.HasWon);
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}