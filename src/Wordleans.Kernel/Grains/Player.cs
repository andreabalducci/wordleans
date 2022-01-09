using Microsoft.Extensions.Logging;
using Orleans;
using Wordleans.Api.Grains;
using Wordleans.Api.Services;

namespace Wordleans.Kernel.Grains;

public class Player : Grain, IPlayer
{
    private readonly IClock _clock;
    private readonly ILogger _logger;

    public Player(IClock clock, ILogger<Player> logger)
    {
        _clock = clock;
        _logger = logger;
    }

    public Task<string> GetTodaysRiddleId()
    {
        var today = _clock.UtcNow().ToString("yyyy-MM-dd");
        var playerId = this.GetPrimaryKeyString();

        var gameId = $"{playerId}/{today}";
        
        _logger.LogInformation("Today's game is {GameId}", gameId);
        
        return Task.FromResult(gameId);
    }
}