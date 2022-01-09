using System.Reflection;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Grains;
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

    public async Task<bool> IsValidWord(string text)
    {
        await Task.Delay(Defaults.SimulatedNetworkDelay);
        var found = this._data!.ContainsWord(text);
        return found;
    }

    public async Task<string> GetRandomWord(int seed)
    {
        await Task.Delay(Defaults.SimulatedNetworkDelay);
        return _data!.GetWordBySeed(seed);
    }

    public async Task<DictionaryData> GetFullDictionary()
    {
        await Task.Delay(Defaults.SimulatedNetworkDelay);
        return _data!;
    }
}
