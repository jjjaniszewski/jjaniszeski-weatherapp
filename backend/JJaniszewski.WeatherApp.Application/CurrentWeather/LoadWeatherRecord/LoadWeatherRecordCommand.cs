using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.LoadWeatherRecord;

/// <summary>
/// Command to load current weather record for a specific city and country.
/// </summary>
public class LoadWeatherRecordCommand : IRequest<Unit>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadWeatherRecordCommand" /> class.
    /// </summary>
    /// <param name="city">The name of the city.</param>
    /// <param name="country">The name of the country.</param>
    public LoadWeatherRecordCommand(string city, string country)
    {
        City = city;
        Country = country;
    }

    /// <summary>
    /// Gets the name of the city for which to load weather data.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the name of the country where the city is located.
    /// </summary>
    public string Country { get; }
}
