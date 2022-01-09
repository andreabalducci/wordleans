using Orleans;

namespace Wordleans.Api.Grains;

public interface IWordRiddle : IGrainWithStringKey
{
    Task<GuessResult> Guess(string word);
    Task<RiddleStatus> GetStatus();
}