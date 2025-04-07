using System;
using Azure;
using Azure.Data.Tables;

namespace JJaniszewski.WeatherApp.Infrastructure.Data;

/// <summary>
/// Represents a table entity for logging weather data load operations
/// </summary>
public class WeatherLoadLogTableEntity : ITableEntity
{
    /// <summary>
    /// Gets or sets the city name for which weather data was loaded
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the country name for which weather data was loaded
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// Gets or sets the status of the weather data load operation
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the date when the weather data was loaded
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Gets or sets the reference to the blob containing the payload data, if any
    /// </summary>
    public string? PayloadBlobReference { get; set; }

    /// <summary>
    /// Gets or sets the reason for the status, if applicable
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets the partition key for the table entity
    /// </summary>
    public string PartitionKey { get; set; }

    /// <summary>
    /// Gets or sets the row key for the table entity
    /// </summary>
    public string RowKey { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the entity in Azure Table Storage
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the ETag for optimistic concurrency
    /// </summary>
    public ETag ETag { get; set; } = ETag.All;
}
