using System.Reflection;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Grains;
public class WordDictionary : Grain, IWordDictionary
{
    private string[] _words = Array.Empty<string>();

    public override async Task OnActivateAsync()
    {
        var id = this.GetPrimaryKeyString();
        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fname = Path.Combine(root, $"{id}.txt");

        var words = await File.ReadAllLinesAsync(fname);
        this._words = words;
    }

    public Task<bool> IsValidWord(string text)
    {
        var found = this._words.Contains(text);
        return Task.FromResult(found);
    }

    public  Task<string> GetRandomWord(int seed)
    {
        var index = seed % _words.Length;
        return Task.FromResult(_words[index]);
    }
}
