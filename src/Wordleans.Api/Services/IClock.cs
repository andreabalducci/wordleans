namespace Wordleans.Api.Services;

public interface IClock
{
    DateTimeOffset UtcNow();
}