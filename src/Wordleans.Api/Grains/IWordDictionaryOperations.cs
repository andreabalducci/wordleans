namespace Wordleans.Api.Grains;

public interface IWordDictionaryOperations
{
    Task<bool> IsValidWord(string text);
    Task<string> GetRandomWord(int seed);
}