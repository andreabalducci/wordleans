using Microsoft.Extensions.Logging;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Grains;

public class WordRiddle : Grain, IWordRiddle
{
    private readonly ILogger<WordRiddle> _logger;
    private IWordDictionary? _dictionary;
    private string _todaysword;
    private DateTimeOffset _contestDay;

    public WordRiddle(ILogger<WordRiddle> logger)
    {
        _logger = logger;
    }

    public override async Task OnActivateAsync()
    {
        _dictionary = GrainFactory.GetGrain<IWordDictionary>(Defaults.DictionaryName);

        var contestDayString = this.GetPrimaryKeyString().Split("/")[1];
        _contestDay = DateTimeOffset.Parse(contestDayString);

        _logger.LogInformation("Contest day is {Day}", _contestDay);

        var seed = _contestDay.DayOfYear + _contestDay.Year * 1000;
        _todaysword = await _dictionary.GetRandomWord(seed);
        _logger.LogInformation("Today's word is {Word}", _todaysword);

        if (String.IsNullOrEmpty(_todaysword))
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
        var isValid = await _dictionary.IsValidWord(word);

        if (!isValid)
        {
            return GuessResult.InvalidWord(word);
        }

        if (word.Length != _todaysword.Length)
        {
            throw new Exception($"Word must be of {_todaysword.Length} letters");
        }

        var matches = new GuessResult.MatchResult[_todaysword.Length];
        
        for (int c = 0; c < _todaysword.Length; c++)
        {
            var currentChar = _todaysword[c];
            if (word[c] == currentChar)
            {
                matches[c] = GuessResult.MatchResult.CorrectSpot;
                continue;
            }

            if (_todaysword.Contains(word[c]))
            {
                matches[c] = GuessResult.MatchResult.WrongSpot;
            }
            else
            {
                matches[c] = GuessResult.MatchResult.NotPresent;
            }
        }
        return GuessResult.ForMatches(word, matches);
    }
}