using System;

namespace JJaniszewski.WeatherApp.Application.Models;

/// <summary>
/// Represents a log entry for weather data loading operations.
/// </summary>
public class WeatherLoadLogEntity : ITableRow
{
    /// <summary>
    /// Initializes a new instance of the WeatherLoadLogEntity class with generated keys.
    /// </summary>
    /// <param name="city">The city for which weather data was loaded.</param>
    /// <param name="country">The country of the city.</param>
    /// <param name="status">The status of the weather data loading operation.</param>
    /// <param name="date">The date and time of the operation.</param>
    /// <param name="payloadBlobReference">Optional reference to the stored payload blob.</param>
    /// <param name="reason">Optional reason or additional information about the operation.</param>
    public WeatherLoadLogEntity(string city, string country, string status, DateTimeOffset date, string? payloadBlobReference = null, string? reason = null)
    {
        RowKey = GenerateRowKey(city, country, date);
        PartitionKey = GeneratePartitionKey(date);
        City = city;
        Country = country;
        Status = status;
        Date = date;
        PayloadBlobReference = payloadBlobReference;
        Reason = reason;
    }

    /// <summary>
    /// Initializes a new instance of the WeatherLoadLogEntity class with provided keys.
    /// </summary>
    /// <param name="rowKey">The row key for the table entry.</param>
    /// <param name="partitionKey">The partition key for the table entry.</param>
    /// <param name="city">The city for which weather data was loaded.</param>
    /// <param name="country">The country of the city.</param>
    /// <param name="status">The status of the weather data loading operation.</param>
    /// <param name="date">The date and time of the operation.</param>
    /// <param name="payloadBlobReference">Optional reference to the stored payload blob.</param>
    /// <param name="reason">Optional reason or additional information about the operation.</param>
    public WeatherLoadLogEntity(string rowKey, string partitionKey, string city, string country, string status, DateTimeOffset date, string? payloadBlobReference = null, string? reason = null)
    {
        RowKey = rowKey;
        PartitionKey = partitionKey;
        City = city;
        Country = country;
        Status = status;
        Date = date;
        PayloadBlobReference = payloadBlobReference;
        Reason = reason;
    }

    /// <summary>
    /// Gets the reason or additional information about the operation, if any.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Gets the city for which weather data was loaded.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the country of the city.
    /// </summary>
    public string Country { get; }

    /// <summary>
    /// Gets the partition key for the table entry.
    /// </summary>
    public string PartitionKey { get; }

    /// <summary>
    /// Gets the row key for the table entry.
    /// </summary>
    public string RowKey { get; }

    /// <summary>
    /// Gets the status of the weather data loading operation.
    /// </summary>
    public string Status { get; }

    /// <summary>
    /// Gets the date and time of the operation.
    /// </summary>
    public DateTimeOffset Date { get; }

    /// <summary>
    /// Gets the reference to the stored payload blob, if any.
    /// </summary>
    public string? PayloadBlobReference { get; }

    /// <summary>
    /// Generates a row key based on city, country and date information.
    /// </summary>
    /// <param name="city">The city name.</param>
    /// <param name="country">The country name.</param>
    /// <param name="date">The date and time.</param>
    /// <returns>A unique row key string.</returns>
    public static string GenerateRowKey(string city, string country, DateTimeOffset date)
    {
        return $"{city}_{country}_{date:yyyyMMddHHmmssfff}_{Guid.NewGuid()}";
    }

    /// <summary>
    /// Generates a partition key based on city, country and date information.
    /// </summary>
    /// <param name="date">The date and time.</param>
    /// <returns>A partition key string.</returns>
    public static string GeneratePartitionKey(DateTimeOffset date)
    {
        return $"WeatherLoadLogs_{date:yyyyMMdd}";
    }
}
