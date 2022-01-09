using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Moq;
using Orleans.TestKit;
using Wordleans.Api.Grains;
using Wordleans.Api.Services;
using Wordleans.Kernel.Grains;
using Xunit;

namespace Wordleans.Tests.GrainsTests;

public class PlayerTests : TestKitBase
{
    public PlayerTests()
    {
        var clock = new FakeClock("2020-01-01");
        Silo.ServiceProvider.AddService<IClock>(clock);
    }

    [Fact]
    public async Task ShouldTrackGames()
    {
        var player = await Silo.CreateGrainAsync<Player>("Player_1");
        var sessionId = await player.GetTodaysRiddleId();

        Assert.Equal("Player_1/2020-01-01", sessionId);
    }

    [Fact]
    public async Task ShouldTrackStats()
    {
        var riddle = Silo.AddProbe<IWordRiddle>("Player_1/2020-01-01");

        var win = GuessResult.ForMatches("CRANK", Enumerable.Repeat(GuessResult.MatchResult.CorrectSpot, 6).ToArray());
        var status = new RiddleStatus()
        {
            Guesses = new[] { win },
            WinningWord = "CRANK"
        };
        
        riddle.Setup(r => r.Guess(It.Is<string>(s => s == "CRANK"))).ReturnsAsync(win);
        riddle.Setup(r => r.GetStatus()).ReturnsAsync(status);

        var player = await Silo.CreateGrainAsync<Player>("Player_1");
        await player.EnterGuess("CRANK");

        var stats = await player.GetStats();
        Assert.Equal(1, stats.Played);
        Assert.Equal(100, stats.PercentWin);
        Assert.Equal(1, stats.GuessDistribution[0]);
        Assert.Equal(0, stats.GuessDistribution[1]);
        Assert.Equal(0, stats.GuessDistribution[2]);
        Assert.Equal(0, stats.GuessDistribution[3]);
        Assert.Equal(0, stats.GuessDistribution[4]);
        Assert.Equal(0, stats.GuessDistribution[5]);
    }
}