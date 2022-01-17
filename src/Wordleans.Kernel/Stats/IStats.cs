using Orleans;

namespace Wordleans.Kernel.Stats;

public interface IStats : IGrainWithGuidKey
{
    Task<long> GetWins();
    Task<long> GetLosses();
}