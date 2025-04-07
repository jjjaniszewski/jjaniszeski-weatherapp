using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogPayload;

/// <summary>
/// Query to get weather load log payload by its ID (row key)
/// </summary>
public class GetWeatherLoadLogPayloadQuery : IRequest<string>
{
    /// <summary>
    /// Initializes a new instance of the GetWeatherLoadLogPayloadQuery class
    /// </summary>
    /// <param name="id">The ID (row key) of the weather load log</param>
    public GetWeatherLoadLogPayloadQuery(string id)
    {
        Id = id;
    }

    /// <summary>
    /// Gets the ID (row key) of the weather load log
    /// </summary>
    public string Id { get; }
}
