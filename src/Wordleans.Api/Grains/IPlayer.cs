using Orleans;

namespace Wordleans.Api.Grains;

public interface IPlayer : IGrainWithStringKey
{
    Task<string> GetTodaysRiddleId();
    Task<GuessResult> EnterGuess(string word);
    Task<PlayerStatistics> GetStats();
}