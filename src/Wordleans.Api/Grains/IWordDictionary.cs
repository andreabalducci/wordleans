using Orleans;

namespace Wordleans.Api.Grains;

public interface IWordDictionary : IGrainWithStringKey, IWordDictionaryOperations
{
    Task<DictionaryData> GetFullDictionary();
}