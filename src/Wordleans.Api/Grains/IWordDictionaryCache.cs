using Orleans;

namespace Wordleans.Api.Grains;

public interface IWordDictionaryCache : IGrainWithStringKey, IWordDictionaryOperations
{
}