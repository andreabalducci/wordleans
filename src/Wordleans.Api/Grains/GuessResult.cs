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

    public string Word { get; private set; }
    public bool IsValidWord { get; private set; }
    public bool HasWon { get; private set; }
    public MatchResult[] Matches { get; private set; } = Array.Empty<MatchResult>();
    public bool GameEnded { get; private set; }

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
        var hasWon = matches.All(x => x == MatchResult.CorrectSpot);
        return new GuessResult()
        {
            IsValidWord = true,
            Word = word,
            Matches = matches,
            HasWon = hasWon,
            GameEnded = hasWon
        };
    }

    public static GuessResult GameOver(string word, MatchResult[] matches)
    {
        return new GuessResult()
        {
            IsValidWord = true,
            Word = word,
            Matches = matches,
            HasWon = false,
            GameEnded = true
        };
    }
}