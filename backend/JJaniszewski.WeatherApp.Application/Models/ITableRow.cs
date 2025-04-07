using System;

namespace JJaniszewski.WeatherApp.Application.Models;

public interface ITableRow
{
    string PartitionKey { get; }

    string RowKey { get; }

    string City { get; }

    string Country { get; }

    string Status { get; }

    DateTimeOffset Date { get; }

    string? PayloadBlobReference { get; }
}
