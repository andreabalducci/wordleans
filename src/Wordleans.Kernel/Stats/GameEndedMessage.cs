using Wordleans.Api.Grains;

namespace Wordleans.Kernel.Stats;

public record GameEndedMessage(string RiddleId, GuessResult Result);