using System;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogPayload;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JJaniszewski.WeatherApp.AzureFunction.Functions;

/// <summary>
/// Azure Function for retrieving weather load log payload
/// </summary>
public class GetWeatherLoadLogPayloadFunction
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the GetWeatherLoadLogPayloadFunction class
    /// </summary>
    /// <param name="mediator">The mediator for handling queries</param>
    public GetWeatherLoadLogPayloadFunction(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// HTTP trigger function to get weather load log payload by ID
    /// </summary>
    /// <param name="req">The HTTP request</param>
    /// <param name="log">The logger</param>
    /// <returns>The payload content</returns>
    [Function("GetWeatherLoadLogPayload")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather-load-logs/{id}/payload")]
        HttpRequest req,
        string id,
        ILogger log)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult("ID parameter is required");
            }

            var query = new GetWeatherLoadLogPayloadQuery(id);
            var result = await _mediator.Send(query);

            return new OkObjectResult(result);
        }
        catch (InvalidOperationException ex)
        {
            log.LogWarning(ex, "Error occurred while getting weather load log payload");
            return new NotFoundObjectResult(ex.Message);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error occurred while getting weather load log payload");
            return new StatusCodeResult(500);
        }
    }
}
