namespace Wordleans.Api.Grains;

public static class Defaults
{
    public const string DictionaryName = "five-letters";
    public const int MaxAttempts = 6;
    public const bool ScaleOutDictionary = false;
    public static TimeSpan SimulatedNetworkDelay { get; } = TimeSpan.FromMilliseconds(200);
}