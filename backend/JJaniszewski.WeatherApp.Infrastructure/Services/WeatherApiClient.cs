using System;
using System.Net.Http;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Options;
using JJaniszewski.WeatherApp.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JJaniszewski.WeatherApp.Infrastructure.Services;

/// <inheritdoc />
public class WeatherApiClient : IWeatherApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherApiClient> _logger;

    private readonly WeatherApiOptions _options;

    public WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger, IOptions<WeatherApiOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public async Task<WeatherLoadResult> GetCurrentWeatherAsync(string city, string country)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city},{country}&appid={_options.ApiKey}";

        var requestDate = DateTimeOffset.UtcNow;
        try
        {
            // var content = "{\"coord\":{\"lon\":-0.1257,\"lat\":51.5085},\"weather\":[{\"id\":800,\"main\":\"Clear\",\"description\":\"clear sky\",\"icon\":\"01d\"}],\"base\":\"stations\",\"main\":{\"temp\":288.16,\"feels_like\":286.79,\"temp_min\":286.48,\"temp_max\":288.81,\"pressure\":1026,\"humidity\":41,\"sea_level\":1026,\"grnd_level\":1022},\"visibility\":10000,\"wind\":{\"speed\":1.34,\"deg\":140,\"gust\":5.36},\"clouds\":{\"all\":6},\"dt\":1744029761,\"sys\":{\"type\":2,\"id\":268730,\"country\":\"GB\",\"sunrise\":1744003317,\"sunset\":1744051371},\"timezone\":3600,\"id\":2643743,\"name\":\"London\",\"cod\":200}";
            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Weather API returned non-success status for {City},{Country}: {StatusCode}", city, country, response.StatusCode);
                return WeatherLoadResult.Failure(city, country, requestDate, content, $"HTTP {response.StatusCode}, content: {content}");
            }

            _logger.LogInformation("Successfully fetched weather data for {City},{Country}", city, country);
            return WeatherLoadResult.Success(city, country, requestDate, content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for {City},{Country}", city, country);
            return WeatherLoadResult.Failure(city, country, requestDate, null, ex.Message);
        }
    }
}
