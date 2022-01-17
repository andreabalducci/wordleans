using Microsoft.Extensions.Logging;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Game;

public class Robot : Grain, IRobot
{
    private readonly ILogger<Robot> _logger;
    private static readonly string[][] Guesses;

    public Robot(ILogger<Robot> logger)
    {
        _logger = logger;
    }

    static Robot()
    {
        Guesses = new[] {
            new[] { "HELLO", "HELLO", "WOUND", "WORLD", "HELLO", "CRANK" },
            new[] { "HELLO", "HELLO", "WOUND", "WORLD", "HELLO", "HELLO" },
            new[] { "WORLD", "CRANK" },
            new[] { "CRANK" }
        };
    }

    public async Task<RobotPlayResult> Play(string playerId, int seed)
    {
        var script = Guess(seed);

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

    private string[] Guess(int seed)
    {
        return Guesses[seed % Guesses.Length];
    }
}