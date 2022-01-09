namespace Wordleans.Api.Grains;

public class PlayerStatistics
{
    public int Played { get; set; }
    public decimal PercentWin { get; set; }
    public int[] GuessDistribution { get; set; } = new int[Defaults.MaxAttempts];
}