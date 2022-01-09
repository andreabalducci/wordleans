namespace Wordleans.Api.Grains;

public class DictionaryData
{
    public DictionaryData(string[] words)
    {
        Words = words;
    }

    public string[] Words { get; private set; } = Array.Empty<string>();

    public bool ContainsWord(string word)
    {
        return Words.Contains(word);
    }

    public string GetWordBySeed(int seed)
    {
        var index = seed % Words.Length;
        return Words[index];
    }
}