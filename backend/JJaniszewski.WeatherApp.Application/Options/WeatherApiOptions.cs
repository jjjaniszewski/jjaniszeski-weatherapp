namespace JJaniszewski.WeatherApp.Application.Options;

/// <summary>
/// Represents the configuration options for the weather API.
/// </summary>
public class WeatherApiOptions
{
    /// <summary>
    /// Gets or sets the API key for the weather API.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
