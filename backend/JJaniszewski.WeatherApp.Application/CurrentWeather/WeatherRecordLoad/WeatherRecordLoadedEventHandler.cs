using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.WeatherRecordLoad;

/// <summary>
/// Handles the WeatherRecordLoaded event by storing weather data and creating load logs
/// </summary>
public class WeatherRecordLoadedEventHandler : INotificationHandler<WeatherRecordLoadedEvent>
{
    private readonly IWeatherLoadLogsTableClient _weatherLoadLogsTableClient;
    private readonly IWeatherRecordPayloadsBlobClient _weatherRecordPayloadsBlobClient;

    /// <summary>
    /// Initializes a new instance of the WeatherRecordLoadedEventHandler
    /// </summary>
    /// <param name="weatherRecordPayloadsBlobClient">Client for storing weather record payloads</param>
    /// <param name="weatherLoadLogsTableClient">Client for storing weather load logs</param>
    public WeatherRecordLoadedEventHandler(IWeatherRecordPayloadsBlobClient weatherRecordPayloadsBlobClient, IWeatherLoadLogsTableClient weatherLoadLogsTableClient)
    {
        _weatherRecordPayloadsBlobClient = weatherRecordPayloadsBlobClient;
        _weatherLoadLogsTableClient = weatherLoadLogsTableClient;
    }

    /// <summary>
    /// Handles the WeatherRecordLoaded event by storing the weather data in blob storage and creating a log entry
    /// </summary>
    /// <param name="notification">The WeatherRecordLoaded event notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task Handle(WeatherRecordLoadedEvent notification, CancellationToken cancellationToken)
    {
        var blobPath = notification.WeatherLoadResult.GetBlobPath();
        await _weatherRecordPayloadsBlobClient.CreateAsync(blobPath, notification.WeatherLoadResult.ResponseData!, cancellationToken);

        var entity = new WeatherLoadLogEntity(notification.City, notification.Country, "Success", notification.WeatherLoadResult.RequestDateUtc, blobPath);

        await _weatherLoadLogsTableClient.CreateAsync(entity, cancellationToken);
    }
}
