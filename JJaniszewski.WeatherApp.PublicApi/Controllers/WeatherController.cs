using Microsoft.AspNetCore.Mvc;

namespace JJaniszewski.WeatherApp.PublicApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{

    private readonly ILogger<WeatherController> _logger;

    public WeatherController(ILogger<WeatherController> logger)
    {
        _logger = logger;
    }

    [HttpGet()]
    public async Task Get()
    {
       
    }
}