using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.Queries.GetAllWeatherRecords;
using JJaniszewski.WeatherApp.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JJaniszewski.WeatherApp.PublicApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherRecordController : ControllerBase
{
    private readonly ILogger<WeatherRecordController> _logger;
    private readonly IMediator _mediator;

    public WeatherRecordController(ILogger<WeatherRecordController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all weather records
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation</param>
    /// <returns>A collection of weather records</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeatherRecordDto>>> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllWeatherRecordsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
