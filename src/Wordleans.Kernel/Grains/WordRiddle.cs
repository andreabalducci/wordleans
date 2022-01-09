using Microsoft.Extensions.Logging;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Grains;

public class WordRiddle : Grain, IWordRiddle
{
    private readonly ILogger<WordRiddle> _logger;
    private IWordDictionaryOperations? _dictionary;
    private string? _winningWord;
    private DateTimeOffset _contestDay;
    private readonly List<GuessResult> _history = new();

    public WordRiddle(ILogger<WordRiddle> logger)
    {
        _logger = logger;
    }

    public override async Task OnActivateAsync()
    {
        if (Defaults.ScaleOutDictionary)
        {
            _dictionary = GrainFactory.GetGrain<IWordDictionaryCache>(Defaults.DictionaryName);
        }
        else
        {
            _dictionary = GrainFactory.GetGrain<IWordDictionary>(Defaults.DictionaryName);
        }


        var contestDayString = this.GetPrimaryKeyString().Split("/")[1];
        _contestDay = DateTimeOffset.Parse(contestDayString);

        _logger.LogInformation("Contest day is {Day}", _contestDay);

        var seed = _contestDay.DayOfYear + _contestDay.Year * 1000;
        _winningWord = await _dictionary.GetRandomWord(seed);
        _logger.LogInformation("Today's word is {Word}", _winningWord);

        if (String.IsNullOrEmpty(_winningWord))
        {
            throw new Exception("System misconfigured");
        }
    }

    public Task<DateTimeOffset> GetGameDay()
    {
        return Task.FromResult(_contestDay);
    }

    public async Task<GuessResult> Guess(string word)
    {
        word = word.ToUpperInvariant();

        // TODO -> slow, move to service grain
        var isValid = await _dictionary!.IsValidWord(word);

        if (!isValid)
        {
            return GuessResult.InvalidWord(word);
        }

        if (word.Length != _winningWord!.Length)
        {
            throw new Exception($"Word must be of {_winningWord!.Length} letters");
        }

        var matches = new GuessResult.MatchResult[_winningWord!.Length];
        
        for (int c = 0; c < _winningWord!.Length; c++)
        {
            var currentChar = _winningWord![c];
            if (word[c] == currentChar)
            {
                matches[c] = GuessResult.MatchResult.CorrectSpot;
                continue;
            }

            if (_winningWord.Contains(word[c]))
            {
                matches[c] = GuessResult.MatchResult.WrongSpot;
            }
            else
            {
                matches[c] = GuessResult.MatchResult.NotPresent;
            }
        }

        bool isLastGuess = _history.Count >= Defaults.MaxAttempts - 1;
        var result = isLastGuess 
            ? GuessResult.GameOver(word, matches) 
            : GuessResult.ForMatches(word, matches);
        
        _history.Add(result);
        return result;
    }

    public Task<RiddleStatus> GetStatus()
    {
        var status = new RiddleStatus()
        {
            Guesses = _history.ToArray(),
            WinningWord = _winningWord!
        };
        return Task.FromResult(status);
    }
}