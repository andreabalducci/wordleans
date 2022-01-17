using System.Reflection;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Game;

public class WordDictionary : Grain, IWordDictionary
{
    private DictionaryData? _data;

    public override async Task OnActivateAsync()
    {
        var id = this.GetPrimaryKeyString();
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fname = Path.Combine(root!, $"{id}.txt");

        var words = await File.ReadAllLinesAsync(fname);
        this._data = new DictionaryData(words);
    }

    public Task<bool> IsValidWord(string text)
    {
        var found = this._data!.ContainsWord(text);
        return Task.FromResult(found);
    }

    public Task<string> GetRandomWord(int seed)
    {
        return Task.FromResult(_data!.GetWordBySeed(seed));
    }

    public Task<DictionaryData> GetFullDictionary()
    {
        return Task.FromResult(_data!);
    }
}