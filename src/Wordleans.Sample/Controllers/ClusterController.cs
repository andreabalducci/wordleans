using Microsoft.AspNetCore.Mvc;
using Orleans;
using Wordleans.Api.Grains;

namespace Wordleans.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class ClusterController : ControllerBase
{
    private readonly ILogger<ClusterController> _logger;
    private readonly IClusterClient _client;
    public ClusterController(ILogger<ClusterController> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpPost("play/{id}")]
    public async Task<string> Play(string id)
    {
        var robot = _client.GetGrain<IRobot>(id);
        var result = await robot.Play(id, 2);
        return result.ToString();
    }

    [HttpPost("run/{robots:int}")]
    public async Task<string> Test(int robots)
    {
        for (int c = 1; c <= robots; c++)
        {
            var robot = _client.GetGrain<IRobot>(c.ToString());
            robot.Play(c.ToString(), c).Ignore();
        }
    
        await Task.Delay(1000);
        return "Started";
    }
}