using Orleans;

namespace Wordleans.Api.Grains;

public interface IWordDictionary : IGrainWithStringKey
{
    Task<bool> IsValidWord(string text);
    Task<string> GetRandomWord(int seed);
}