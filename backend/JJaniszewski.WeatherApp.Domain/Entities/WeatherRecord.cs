using System;

namespace JJaniszewski.WeatherApp.Domain.Entities;

/// <summary>
/// Represents a weather record containing meteorological data for a specific location and time.
/// </summary>
public class WeatherRecord
{
    private WeatherRecord() { }

    /// <summary>
    /// Gets the unique identifier of the weather record.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the name of the city for which the weather data was recorded.
    /// </summary>
    public string City { get; private set; }

    /// <summary>
    /// Gets the name of the country where the city is located.
    /// </summary>
    public string Country { get; private set; }

    /// <summary>
    /// Gets the date and time in UTC when the weather data was recorded.
    /// </summary>
    public DateTimeOffset DateUtc { get; private set; }

    /// <summary>
    /// Gets the date and time in UTC when the weather data was requested.
    /// </summary>
    public DateTimeOffset RequestDateUtc { get; private set; }

    /// <summary>
    /// Gets the minimum temperature at current time in Kelvin recorded.
    /// Used as in bigger cities temperature can differ much based on region.
    /// </summary>
    public double MinTemperature { get; private set; }

    /// <summary>
    /// Gets the maximum temperature at current time in Kelvin recorded.
    /// /// Used as in bigger cities temperature can differ much based on region.
    /// </summary>
    public double MaxTemperature { get; private set; }

    /// <summary>
    /// Gets the current temperature in Kelvin.
    /// </summary>
    public double Temperature { get; private set; }

    /// <summary>
    /// Creates a new instance of the WeatherRecord class with the specified parameters.
    /// </summary>
    /// <param name="city">The name of the city.</param>
    /// <param name="country">The name of the country.</param>
    /// <param name="dateUtc">The date and time in UTC when the weather data was recorded.</param>
    /// <param name="requestDateUtc">The date and time in UTC when the weather data was requested.</param>
    /// <param name="temperature">The current temperature in Kelvin.</param>
    /// <param name="minTemperature">The minimum temperature at current time in Kelvin.</param>
    /// <param name="maxTemperature">The maximum temperature at current time in Kelvin.</param>
    /// <returns>A new instance of WeatherRecord.</returns>
    public static WeatherRecord Create(string city, string country, DateTimeOffset dateUtc, DateTimeOffset requestDateUtc, double temperature, double minTemperature, double maxTemperature)
    {
        return new WeatherRecord
        {
            Id = Guid.NewGuid(),
            City = city,
            Country = country,
            DateUtc = dateUtc,
            RequestDateUtc = requestDateUtc,
            Temperature = temperature,
            MinTemperature = minTemperature,
            MaxTemperature = maxTemperature
        };
    }
}
