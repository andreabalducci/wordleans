namespace Wordleans.Api.Services;

public class SystemClock : IClock
{
    public static IClock Instance = new SystemClock();
    
    public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
}