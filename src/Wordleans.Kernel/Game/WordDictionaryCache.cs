using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Game;

[StatelessWorker]
public class WordDictionaryCache : Grain, IWordDictionaryCache
{
    private static int _cacheCounter = 0;
    private DictionaryData? _data;
    private readonly ILogger _logger;

    public WordDictionaryCache(ILoggerFactory loggerFactory, ILocalSiloDetails localSiloDetails)
    {
        var id = Interlocked.Increment(ref _cacheCounter);
        var silo = localSiloDetails.SiloAddress.Endpoint.ToString();
        
        _logger = loggerFactory.CreateLogger($"{GetType().FullName}.{id:D3}@{silo}");
    }

    public override async Task OnActivateAsync()
    {
        _logger.LogInformation("Activating new dictionary cache");
        var master = GrainFactory.GetGrain<IWordDictionary>(Defaults.DictionaryName);
        this._data = await master.GetFullDictionary();
    }

    public Task<bool> IsValidWord(string text)
    {
        var found = this._data!.ContainsWord(text);
        return Task.FromResult(found);
    }

    public  Task<string> GetRandomWord(int seed)
    {
        return Task.FromResult(_data!.GetWordBySeed(seed));
    }
}