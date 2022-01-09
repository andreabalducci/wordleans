using System;
using Wordleans.Api.Services;

namespace Wordleans.Tests;

public class FakeClock : IClock
{
    private DateTimeOffset _now = DateTimeOffset.UtcNow;

    public FakeClock()
    {
        
    }

    public FakeClock(string dateTime)
    {
        Set(dateTime);
    }
    
    public void Set(string datetime)
    {
        _now = DateTimeOffset.Parse(datetime);
    }

    public DateTimeOffset UtcNow() => _now;
}