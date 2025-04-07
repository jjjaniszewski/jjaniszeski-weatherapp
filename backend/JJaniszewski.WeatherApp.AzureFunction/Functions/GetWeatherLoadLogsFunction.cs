using System;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JJaniszewski.WeatherApp.AzureFunction.Functions;

/// <summary>
/// Azure Function for retrieving weather load logs
/// </summary>
public class GetWeatherLoadLogsFunction
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the GetWeatherLoadLogsFunction class
    /// </summary>
    /// <param name="mediator">The mediator for handling queries</param>
    public GetWeatherLoadLogsFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// HTTP trigger function to get weather load logs within a specified date range
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <param name="log">The logger</param>
    /// <returns>A collection of weather load log entities</returns>
    [Function("GetWeatherLoadLogs")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather-load-logs")]
        HttpRequest req,
        ILogger log)
    {
        try
        {
            // Parse query parameters
            if (!DateTimeOffset.TryParse(req.Query["fromDate"], out var fromDate))
            {
                return new BadRequestObjectResult("fromDate parameter is required and must be a valid date");
            }

            if (!DateTimeOffset.TryParse(req.Query["toDate"], out var toDate))
            {
                return new BadRequestObjectResult("toDate parameter is required and must be a valid date");
            }

            if (fromDate > toDate)
            {
                return new BadRequestObjectResult("fromDate must be less than or equal to toDate");
            }

            var query = new GetWeatherLoadLogsQuery(fromDate, toDate);
            var result = await _mediator.Send(query);

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error occurred while getting weather load logs");
            return new StatusCodeResult(500);
        }
    }
}
