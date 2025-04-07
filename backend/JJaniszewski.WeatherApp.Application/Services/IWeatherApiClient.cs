using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Models;

namespace JJaniszewski.WeatherApp.Application.Services;

/// <summary>
/// Represents a client for interacting with the weather API.
/// </summary>
public interface IWeatherApiClient
{
    /// <summary>
    /// Retrieves the current weather data for a specific city and country.
    /// </summary>
    Task<WeatherLoadResult> GetCurrentWeatherAsync(string city, string country);
}
