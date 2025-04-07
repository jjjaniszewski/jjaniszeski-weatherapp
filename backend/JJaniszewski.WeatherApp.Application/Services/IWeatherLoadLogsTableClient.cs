using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JJaniszewski.WeatherApp.Application.Models;

namespace JJaniszewski.WeatherApp.Application.Services;

/// <summary>
/// Interface for interacting with weather load logs table storage
/// </summary>
public interface IWeatherLoadLogsTableClient
{
    /// <summary>
    /// Creates a new weather load log entry in table storage
    /// </summary>
    /// <param name="entity">The weather load log entity to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task CreateAsync(WeatherLoadLogEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a specific weather load log entry from table storage by its row key
    /// </summary>
    /// <param name="rowKey">The row key of the entry</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested weather load log entity</returns>
    Task<WeatherLoadLogEntity?> GetAsync(string rowKey, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a specific weather load log entry from table storage
    /// </summary>
    /// <param name="partitionKey">The partition key of the entry</param>
    /// <param name="rowKey">The row key of the entry</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested weather load log entity</returns>
    Task<WeatherLoadLogEntity?> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

    /// <summary>
    /// Queries weather load log entries by partition key
    /// </summary>
    /// <param name="partitionKey">The partition key to query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of weather load log entities</returns>
    Task<IEnumerable<WeatherLoadLogEntity>> QueryAsync(string partitionKey, CancellationToken cancellationToken);

    /// <summary>
    /// Queries weather load log entries by date range
    /// </summary>
    /// <param name="fromDate">The start date of the range</param>
    /// <param name="toDate">The end date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of weather load log entities within the specified date range</returns>
    Task<IEnumerable<WeatherLoadLogEntity>> QueryByDateRangeAsync(DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken);
}
