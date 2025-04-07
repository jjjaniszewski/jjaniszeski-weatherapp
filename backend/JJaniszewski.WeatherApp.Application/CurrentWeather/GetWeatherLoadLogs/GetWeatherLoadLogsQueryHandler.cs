using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Services;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogs;

/// <summary>
/// Handler for the GetWeatherLoadLogsQuery
/// </summary>
public class GetWeatherLoadLogsQueryHandler : IRequestHandler<GetWeatherLoadLogsQuery, IEnumerable<WeatherLoadLogResponse>>
{
    private readonly IWeatherLoadLogsTableClient _weatherLoadLogsTableClient;

    /// <summary>
    /// Initializes a new instance of the GetWeatherLoadLogsQueryHandler class
    /// </summary>
    /// <param name="weatherLoadLogsTableClient">The client for accessing weather load logs table storage</param>
    public GetWeatherLoadLogsQueryHandler(IWeatherLoadLogsTableClient weatherLoadLogsTableClient)
    {
        _weatherLoadLogsTableClient = weatherLoadLogsTableClient;
    }

    /// <summary>
    /// Handles the query to get weather load logs within a specified date range
    /// </summary>
    /// <param name="request">The query containing the date range</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A collection of weather load log results within the specified date range</returns>
    public async Task<IEnumerable<WeatherLoadLogResponse>> Handle(GetWeatherLoadLogsQuery request, CancellationToken cancellationToken)
    {
        var entities = await _weatherLoadLogsTableClient.QueryByDateRangeAsync(request.FromDate, request.ToDate, cancellationToken);

        return entities.Select(entity => new WeatherLoadLogResponse(
            entity.RowKey,
            entity.City,
            entity.Country,
            entity.Status,
            entity.Date,
            entity.Reason));
    }
}
