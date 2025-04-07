using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Data.Tables;
using JJaniszewski.WeatherApp.Application.Models;
using JJaniszewski.WeatherApp.Application.Services;
using JJaniszewski.WeatherApp.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace JJaniszewski.WeatherApp.Infrastructure.Services;

/// <inheritdoc />
public class AzureWeatherLoadLogsTableClient : IWeatherLoadLogsTableClient
{
    public const string TABLE_NAME = "WeatherLoadLogs";
    private readonly ILogger<AzureWeatherLoadLogsTableClient> _logger;
    private readonly TableClient _tableClient;

    public AzureWeatherLoadLogsTableClient(TableClient tableClient, ILogger<AzureWeatherLoadLogsTableClient> logger)
    {
        _tableClient = tableClient ?? throw new ArgumentNullException(nameof(tableClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task CreateAsync(WeatherLoadLogEntity entity, CancellationToken cancellationToken)
    {
        try
        {
            var tableEntity = ConvertToTableEntity(entity);
            await _tableClient.AddEntityAsync(tableEntity, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity in table storage");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<WeatherLoadLogEntity?> GetAsync(string rowKey, CancellationToken cancellationToken)
    {
        try
        {
            // Query the table to find the entity by row key
            var query = _tableClient.QueryAsync<TableEntity>($"RowKey eq '{rowKey}'", cancellationToken: cancellationToken);

            await foreach (var entity in query)
            {
                return ConvertFromTableEntity(entity);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity from table storage");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<WeatherLoadLogEntity?> GetAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<TableEntity>(partitionKey, rowKey, cancellationToken: cancellationToken);
            return response.HasValue ? ConvertFromTableEntity(response.Value) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity from table storage");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WeatherLoadLogEntity>> QueryAsync(string partitionKey, CancellationToken cancellationToken)
    {
        try
        {
            var query = _tableClient.QueryAsync<TableEntity>($"PartitionKey eq '{partitionKey}'", cancellationToken: cancellationToken);
            var result = new List<WeatherLoadLogEntity>();

            await foreach (var entity in query)
            {
                result.Add(ConvertFromTableEntity(entity));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying entities from table storage");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<WeatherLoadLogEntity>> QueryByDateRangeAsync(DateTimeOffset fromDate, DateTimeOffset toDate, CancellationToken cancellationToken)
    {
        try
        {
            var currentDate = fromDate.Date;
            var result = new List<WeatherLoadLogEntity>();

            while (currentDate <= toDate.Date)
            {
                var partitionKey = WeatherLoadLogEntity.GeneratePartitionKey(currentDate);
                var filter = $"PartitionKey eq '{partitionKey}' and Date ge datetime'{fromDate:yyyy-MM-ddTHH:mm:ss.fffZ}' and Date le datetime'{toDate:yyyy-MM-ddTHH:mm:ss.fffZ}'";

                var query = _tableClient.QueryAsync<TableEntity>(filter, cancellationToken: cancellationToken);

                await foreach (var entity in query)
                {
                    result.Add(ConvertFromTableEntity(entity));
                }

                currentDate = currentDate.AddDays(1);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying entities by date range from table storage");
            throw;
        }
    }

    private static WeatherLoadLogTableEntity ConvertToTableEntity(WeatherLoadLogEntity row)
    {
        return new WeatherLoadLogTableEntity
        {
            PartitionKey = row.PartitionKey,
            RowKey = row.RowKey,
            City = row.City,
            Country = row.Country,
            Status = row.Status,
            Date = row.Date,
            PayloadBlobReference = row.PayloadBlobReference,
            Reason = row.Reason
        };
    }

    private static WeatherLoadLogEntity ConvertFromTableEntity(TableEntity entity)
    {
        return new WeatherLoadLogEntity(
            entity.RowKey,
            entity.PartitionKey,
            entity.GetString("City") ?? string.Empty,
            entity.GetString("Country") ?? string.Empty,
            entity.GetString("Status") ?? string.Empty,
            entity.GetDateTimeOffset("Date") ?? DateTimeOffset.UtcNow,
            entity.GetString("PayloadBlobReference"),
            entity.GetString("Reason"));
    }
}
