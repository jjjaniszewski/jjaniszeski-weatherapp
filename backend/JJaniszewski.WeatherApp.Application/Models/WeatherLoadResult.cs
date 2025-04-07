using System;
using System.Text.Json;

namespace JJaniszewski.WeatherApp.Application.Models;

/// <summary>
/// Represents the result of loading weather data for a specific city and country.
/// </summary>
public class WeatherLoadResult
{
    /// <summary>
    /// JSON serializer options for deserializing weather API responses.
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherLoadResult" /> class.
    /// </summary>
    private WeatherLoadResult(string city, string country, DateTimeOffset requestDateUtc, bool isSuccess, string? responseData, string? errorMessage, WeatherApiResponse? parsedResponse)
    {
        City = city;
        Country = country;
        RequestDateUtc = requestDateUtc;
        IsSuccess = isSuccess;
        ResponseData = responseData;
        ErrorMessage = errorMessage;
        ParsedResponse = parsedResponse;
    }

    /// <summary>
    /// Gets the name of the city for which weather data was requested.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the country code for which weather data was requested.
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Gets the UTC timestamp when the weather data request was made.
    /// </summary>
    public DateTimeOffset RequestDateUtc { get; }

    /// <summary>
    /// Gets a value indicating whether the weather data load operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the raw response data received from the weather API.
    /// </summary>
    public string? ResponseData { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the parsed weather API response object.
    /// </summary>
    public WeatherApiResponse? ParsedResponse { get; }

    /// <summary>
    /// Gets the blob storage path for the current weather load result.
    /// </summary>
    /// <returns>A string representing the blob storage path.</returns>
    public string GetBlobPath()
    {
        return GetBlobPath(City, Country, RequestDateUtc);
    }

    /// <summary>
    /// Generates a blob storage path for weather data based on city, country and request time.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country code.</param>
    /// <param name="requestDateUtc">The UTC timestamp of the request.</param>
    /// <returns>A string representing the blob storage path.</returns>
    public static string GetBlobPath(string city, string country, DateTimeOffset requestDateUtc)
    {
        return $"payloads/{city}-{country}-{requestDateUtc:yyyy-MM-dd_HH-mm-ss-fff}.json";
    }

    /// <summary>
    /// Creates a successful weather load result.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country code.</param>
    /// <param name="requestDateUtc">The UTC timestamp of the request.</param>
    /// <param name="responseData">The raw response data from the weather API.</param>
    /// <returns>A new instance of <see cref="WeatherLoadResult" /> representing a successful operation.</returns>
    public static WeatherLoadResult Success(string city, string country, DateTimeOffset requestDateUtc, string responseData)
    {
        var parsedResponse = JsonSerializer.Deserialize<WeatherApiResponse>(responseData, JsonOptions);
        return new WeatherLoadResult(city, country, requestDateUtc, true, responseData, null, parsedResponse);
    }

    /// <summary>
    /// Creates a failed weather load result.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country code.</param>
    /// <param name="requestDateUtc">The UTC timestamp of the request.</param>
    /// <param name="responseData">The raw response data from the weather API, if any.</param>
    /// <param name="error">The error message describing the failure.</param>
    /// <returns>A new instance of <see cref="WeatherLoadResult" /> representing a failed operation.</returns>
    public static WeatherLoadResult Failure(string city, string country, DateTimeOffset requestDateUtc, string? responseData, string error)
    {
        return new WeatherLoadResult(city, country, requestDateUtc, false, responseData, error, null);
    }
}
