using Microsoft.Extensions.Logging;
using Orleans;
using Wordleans.Api.Grains;
using Wordleans.Api.Services;
using Wordleans.Kernel.Stats;

namespace Wordleans.Kernel.Game;

public class Player : Grain, IPlayer
{
    private readonly IClock _clock;
    private readonly ILogger _logger;
    private readonly PlayerStatistics _stats = new();
    public Player(IClock clock, ILogger<Player> logger)
    {
        _clock = clock;
        _logger = logger;
    }

    private string GetCurrentRiddleId()
    {
        var today = _clock.UtcNow().ToString("yyyy-MM-dd");
        var playerId = this.GetPrimaryKeyString();

        var riddleId = $"{playerId}/{today}";
        
        _logger.LogDebug("Today's game is {RiddleId}", riddleId);

        return riddleId;
    }

    public Task<string> GetTodaysRiddleId()
    {
        return Task.FromResult(GetCurrentRiddleId());
    }

    public async Task<GuessResult> EnterGuess(string word)
    {
        var id = GetCurrentRiddleId();
        var riddle = GrainFactory.GetGrain<IWordRiddle>(id);
        var result = await riddle.Guess(word);

        if (result.GameEnded)
        {
            _stats.Played++;
            
            if (result.HasWon)
            {
                var gameStatus = await riddle.GetStatus();
                var attempts = gameStatus.Guesses.Length;
                _stats.GuessDistribution[attempts - 1]++;
            }

            var totalWon = _stats.GuessDistribution.Sum(x => x);
            _stats.PercentWin = totalWon * 100.0M / _stats.Played;
        }

        await TrackGameEnd(id, result);

        return result;
    }

    private async Task TrackGameEnd(string riddleId, GuessResult result)
    {
        if (!result.GameEnded)
        {
            return;
        }

        var provider = GetStreamProvider(StatsDefaults.StatsProvider);
        var msg = new GameEndedMessage(riddleId, result);
        await provider.GetStream<GameEndedMessage>(StatsDefaults.GrainId, StatsDefaults.Namespace)
            .OnNextAsync(msg);
    }

    public Task<PlayerStatistics> GetStats()
    {
        return Task.FromResult(_stats);
    }
}