using System.Threading.Tasks;
using Orleans.TestKit;
using Wordleans.Kernel.Grains;
using Xunit;

namespace Wordleans.Tests.GrainsTests;

public class WordDictionaryTests : TestKitBase
{
    [Fact]
    public async Task ShouldLoadWordDictionary()
    {
        var dictionary = await Silo.CreateGrainAsync<WordDictionary>("five-letters");
        var found = await dictionary.IsValidWord("HELLO");
        Assert.True(found);
    }

    [Theory]
    [InlineData(0, "CRANK")]
    [InlineData(1, "HELLO")]
    [InlineData(2, "WORLD")]
    [InlineData(3, "RISKY")]
    [InlineData(4, "WINDY")]
    [InlineData(5, "WOUND")]
    [InlineData(6, "CRANK")]
    [InlineData(42, "CRANK")]
    public async Task ShouldSelectWordBySeed(int seed, string expected)
    {
        var dictionary = await Silo.CreateGrainAsync<WordDictionary>("five-letters");
        var word = await dictionary.GetRandomWord(seed);
        Assert.Equal(expected, word);
    }

    [Fact]
    public async Task ShouldSelectSameWordForSameSeed()
    {
        var dictionary = await Silo.CreateGrainAsync<WordDictionary>("five-letters");
        var selected1 = await dictionary.GetRandomWord(42);
        var selected2 = await dictionary.GetRandomWord(42);
        Assert.Equal(selected2, selected1);
    }
}