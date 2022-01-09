using Orleans;

namespace Wordleans.Api.Grains;

public static class Defaults
{
    public const string DictionaryName = "five-letters";
}

public interface IWordDictionary : IGrainWithStringKey
{
    Task<bool> IsValidWord(string text);
    Task<string> GetRandomWord(int seed);
}