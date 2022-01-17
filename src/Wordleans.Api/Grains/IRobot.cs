using Orleans;

namespace Wordleans.Api.Grains;

public enum RobotPlayResult
{
    Won,
    Lost,
    Playing
}

public interface IRobot : IGrainWithStringKey
{
    Task<RobotPlayResult> Play(string playerId, int seed);
}