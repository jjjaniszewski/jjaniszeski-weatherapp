using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.WeatherRecordLoad;

/// <summary>
/// Handles the WeatherRecordLoadFailureEvent by creating a log entry in the weather load logs table
/// </summary>
public class WeatherRecordLoadFailureEventHandler : INotificationHandler<WeatherRecordLoadFailureEvent>
{
    private readonly IWeatherLoadLogsTableClient _weatherLoadLogsTableClient;

    /// <summary>
    /// Initializes a new instance of the WeatherRecordLoadFailureEventHandler
    /// </summary>
    /// <param name="weatherLoadLogsTableClient">Client for storing weather load logs</param>
    public WeatherRecordLoadFailureEventHandler(IWeatherLoadLogsTableClient weatherLoadLogsTableClient)
    {
        _weatherLoadLogsTableClient = weatherLoadLogsTableClient;
    }

    /// <summary>
    /// Handles the WeatherRecordLoadFailureEvent by creating a log entry in the weather load logs table
    /// </summary>
    /// <param name="notification">The WeatherRecordLoadFailureEvent notification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task Handle(WeatherRecordLoadFailureEvent notification, CancellationToken cancellationToken)
    {
        var entity = new WeatherLoadLogEntity(
            notification.City,
            notification.Country,
            "Failure",
            notification.WeatherLoadResult.RequestDateUtc,
            reason: notification.WeatherLoadResult.ErrorMessage);

        await _weatherLoadLogsTableClient.CreateAsync(entity, cancellationToken);
    }
}
