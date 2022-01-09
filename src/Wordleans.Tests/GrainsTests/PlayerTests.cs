using System.Threading.Tasks;
using Orleans.TestKit;
using Wordleans.Api.Services;
using Wordleans.Kernel.Grains;
using Xunit;

namespace Wordleans.Tests.GrainsTests;

public class PlayerTests : TestKitBase
{
    [Fact]
    public async Task ShouldTrackGames()
    {
        var clock = new FakeClock("2020-01-01");
        Silo.ServiceProvider.AddService<IClock>(clock);
        var player = await Silo.CreateGrainAsync<Player>("Player_1");
        
        var sessionId = await player.GetTodaysRiddleId();
        
        Assert.Equal("Player_1/2020-01-01", sessionId);
    }
}