using Orleans;

namespace Wordleans.Api.Grains;


[Serializable]
public class GuessResult
{
    public enum MatchResult
    {
        NotPresent,
        WrongSpot,
        CorrectSpot
    }

    public string Word { get; set; }
    public bool IsValidWord { get; private set; }
    public bool HasWon { get; private set; }
    public MatchResult[] Matches { get; set; } = Array.Empty<MatchResult>();

    public static GuessResult InvalidWord(string word)
    {
        return new GuessResult()
        {
            IsValidWord = false,
            Word = word
        };
    }

    public static GuessResult ForMatches(string word, MatchResult[] matches)
    {
        return new GuessResult()
        {
            IsValidWord = true,
            Word = word,
            Matches = matches,
            HasWon = matches.All(x=>x == MatchResult.CorrectSpot)
        };
    }
}

public interface IWordRiddle : IGrainWithStringKey
{
    Task<GuessResult> Guess(string word);
}