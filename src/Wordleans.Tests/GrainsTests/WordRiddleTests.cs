using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Orleans.TestKit;
using Wordleans.Api.Grains;
using Wordleans.Kernel.Grains;
using Xunit;

namespace Wordleans.Tests.GrainsTests;

public class WordRiddleTests : TestKitBase
{
    private readonly Mock<IWordDictionary>? _dictionary;

    public WordRiddleTests()
    {
        _dictionary = Silo.AddProbe<IWordDictionary>(Defaults.DictionaryName);
    }

    [Fact]
    public async Task ShouldCreateRiddleByDate()
    {
        _dictionary.Setup(x => x.GetRandomWord(It.IsAny<int>())).ReturnsAsync("XWORD");
        var riddle = await Silo.CreateGrainAsync<WordRiddle>("game/2020-01-01");
        var gameDay = await riddle.GetGameDay();

        Assert.Equal(DateTimeOffset.Parse("2020-01-01"), gameDay);
    }

    [Fact]
    public async Task ShouldGuessWord()
    {
        _dictionary.Setup(x => x.GetRandomWord(It.IsAny<int>())).ReturnsAsync("XWORD");
        _dictionary.Setup(x => x.IsValidWord(It.IsAny<string>())).ReturnsAsync((string g) => g == "XWORD");

        var riddle = await Silo.CreateGrainAsync<WordRiddle>("game/2020-01-01");

        var result = await riddle.Guess("XWORD");
        Assert.True(result.HasWon);
    }

    [Fact]
    public async Task ShouldDetectInvalidWord()
    {
        _dictionary.Setup(x => x.GetRandomWord(It.IsAny<int>())).ReturnsAsync("XWORD");
        _dictionary.Setup(x => x.IsValidWord(It.IsAny<string>())).ReturnsAsync((string g) => g == "XWORD");

        var riddle = await Silo.CreateGrainAsync<WordRiddle>("game/2020-01-01");

        var result = await riddle.Guess("1WORD");
        Assert.False(result.IsValidWord);
    }

    [Theory]
    [InlineData("CDEAB", "ZULFX", "!!!!!")]
    [InlineData("CDEAB", "ABCDE", "xxxxx")]
    [InlineData("ABCDE", "ABCDE", "ooooo")]
    [InlineData("ABCDE", "BBCDX", "xooo!")]
    public async Task ShouldMatch(string expected, string guess, string matchSummary)
    {
        _dictionary.Setup(x => x.GetRandomWord(It.IsAny<int>())).ReturnsAsync(expected);
        _dictionary.Setup(x => x.IsValidWord(It.IsAny<string>())).ReturnsAsync(true);

        var riddle = await Silo.CreateGrainAsync<WordRiddle>("game/2020-01-01");

        var result = await riddle.Guess(guess);
        var matches = matchSummary.Select(x => x switch
            {
                'x' => GuessResult.MatchResult.WrongSpot,
                '!' => GuessResult.MatchResult.NotPresent,
                'o' => GuessResult.MatchResult.CorrectSpot,
                _ => throw new Exception($"Invalid option '{x}'")
            }
        ).ToArray();

        Assert.Equal(matches, result.Matches);
    }
}