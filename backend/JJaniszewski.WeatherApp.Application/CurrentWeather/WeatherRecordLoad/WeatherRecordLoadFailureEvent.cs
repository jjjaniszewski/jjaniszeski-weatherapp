using JJaniszewski.WeatherApp.Application.Models;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.WeatherRecordLoad;

/// <summary>
/// Event raised when weather data loading fails for a specific city and country.
/// </summary>
public class WeatherRecordLoadFailureEvent : INotification
{
    /// <summary>
    /// Initializes a new instance of the WeatherRecordLoadFailureEvent class.
    /// </summary>
    /// <param name="city">The name of the city.</param>
    /// <param name="country">The name of the country.</param>
    /// <param name="weatherLoadResult">The result of the weather data loading operation.</param>
    public WeatherRecordLoadFailureEvent(string city, string country, WeatherLoadResult weatherLoadResult)
    {
        City = city;
        Country = country;
        WeatherLoadResult = weatherLoadResult;
    }

    /// <summary>
    /// Gets the name of the city for which weather data loading failed.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the name of the country where the city is located.
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Gets the result of the weather data loading operation.
    /// </summary>
    public WeatherLoadResult WeatherLoadResult { get; }
}
