using System;
using System.Collections.Generic;
using MediatR;

namespace JJaniszewski.WeatherApp.Application.CurrentWeather.GetWeatherLoadLogs;

/// <summary>
/// Query to get weather load logs within a specified date range
/// </summary>
public class GetWeatherLoadLogsQuery : IRequest<IEnumerable<WeatherLoadLogResponse>>
{
    /// <summary>
    /// Initializes a new instance of the GetWeatherLoadLogsQuery class
    /// </summary>
    /// <param name="fromDate">The start date of the range</param>
    /// <param name="toDate">The end date of the range</param>
    public GetWeatherLoadLogsQuery(DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        FromDate = fromDate;
        ToDate = toDate;
    }

    /// <summary>
    /// Gets the start date of the range
    /// </summary>
    public DateTimeOffset FromDate { get; }

    /// <summary>
    /// Gets the end date of the range
    /// </summary>
    public DateTimeOffset ToDate { get; }
}
