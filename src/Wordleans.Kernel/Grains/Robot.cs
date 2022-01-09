using Microsoft.Extensions.Logging;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Grains;

public class Robot : Grain, IRobot
{
    private ILogger<Robot> _logger;

    public Robot(ILogger<Robot> logger)
    {
        _logger = logger;
    }

    public async Task<RobotPlayResult> Play(string playerId, string[] script)
    {
        _logger.LogDebug("Playing for {PlayerId} ({@Script})", playerId, script);
        var player = GrainFactory.GetGrain<IPlayer>(playerId);
        
        foreach (var guess in script)
        {
            var result = await player.EnterGuess(guess);
            if (result.GameEnded)
            {
                return result.HasWon ? RobotPlayResult.Won : RobotPlayResult.Lost;
            }
        }

        return RobotPlayResult.Playing;
    }
}