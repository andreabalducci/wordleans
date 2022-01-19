using Microsoft.AspNetCore.Mvc;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class PlayController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<PlayController> _logger;
    private readonly IClusterClient _client;
    public PlayController(ILogger<PlayController> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpPost(Name = "Play/{id}")]
    public async Task<string> Play(string id)
    {
        var robot = _client.GetGrain<IRobot>(id);
        var result = await robot.Play(id, 2);
        return result.ToString();
    }
}