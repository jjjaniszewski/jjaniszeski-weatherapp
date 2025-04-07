using System;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogs;

/// <summary>
/// Represents a weather load log response
/// </summary>
public class WeatherLoadLogResponse
{
    /// <summary>
    /// Initializes a new instance of the WeatherLoadLogResult class
    /// </summary>
    public WeatherLoadLogResponse(string id, string city, string country, string status, DateTimeOffset date, string? reason)
    {
        City = city;
        Country = country;
        Id = id;
        Status = status;
        Date = date;
        Reason = reason;
    }

    /// <summary>
    /// Gets the city name for which weather data was loaded
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the country name for which weather data was loaded
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Gets the Id which is row key for the table entity
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the status of the weather data load operation
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the date when the weather data was loaded
    /// </summary>
    public DateTimeOffset Date { get; }

    /// <summary>
    /// Gets the reason for the status, if applicable
    /// </summary>
    public string? Reason { get; }
}
