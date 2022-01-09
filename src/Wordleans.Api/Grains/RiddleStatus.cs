namespace Wordleans.Api.Grains;

public class RiddleStatus
{
    public string WinningWord { get; set; }
    public GuessResult[] Guesses { get; set; }
}